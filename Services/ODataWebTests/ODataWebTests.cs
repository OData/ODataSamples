// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Services.Client;
using ODataWebTests.ODataDemoModel;
using System.Net;

namespace ODataWebTests
{
    [TestClass]
    public class ODataWebSessionTest
    {
        private DataServiceContext ctx;
        private int nextId = 100;

        [TestInitialize()]
        public void TestInitialize()
        {
            CreateSession();
        }

        private void CreateSession()
        {
            string sessionId = GenerateSessionId();
#if DEBUG
            Uri sessionedUri = new Uri("http://localhost:51514/V2/(S(" + sessionId + "))/OData/OData.svc/", UriKind.Absolute);
#else
            Uri sessionedUri = new Uri("http://services.odata.org/V2/(S(" + sessionId + "))/OData/OData.svc/", UriKind.Absolute);
#endif
            ctx = new DataServiceContext(sessionedUri);
        }

        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void EntityCountAcrossSession()
        {
            VerifyEntityCount(9, 3);
            CreateEntity(false);

            CreateSession();
            VerifyEntityCount(9, 3);
        }

        [TestMethod]
        public void EntityCountSameSession()
        {
            VerifyEntityCount(9, 3);
            CreateEntity(true);
            VerifyEntityCount(10, 4);
        }

        [TestMethod]
        public void InsertLargeQuantity_Products()
        {
            VerifyContainerLimit("Products", CreateProduct);
        }

        [TestMethod]
        public void InsertLargeQuantity_Categories()
        {
            VerifyContainerLimit("Categories", CreateCategory);
        }

        [TestMethod]
        public void InsertLengthyStringTest_Post()
        {
            Category newCate = CreateCategory();

            for (int i = 0; i < 26; ++i)
            {
                newCate.Name += "0123456789";
            }

            ctx.AddObject("Categories", newCate);
            try
            {
                ctx.SaveChanges();
                Assert.Fail("Large string failed to throw");
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("String property \"Name\" on this service has a maximum length restriction of 256 characters."));
            }
        }

        [TestMethod]
        public void UpdateEntityTest_SessionLess()
        {
            ctx.MergeOption = MergeOption.OverwriteChanges;
            Category cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            cate.Name = "something else";
            ctx.UpdateObject(cate);
            ctx.SaveChanges();

            CreateSession();
            cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            Assert.AreEqual("Food", cate.Name);
        }

        [TestMethod]
        public void UpdateEntityTest_Sessioned()
        {
            ctx.MergeOption = MergeOption.OverwriteChanges;
            Category cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            cate.Name = "something else";
            ctx.UpdateObject(cate);
            ctx.SaveChanges();

            cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            Assert.AreEqual("something else", cate.Name);
        }

        [TestMethod]
        public void UpdateEntityTest_ETag()
        {
            Supplier s = this.CreateSupplier();
            ctx.MergeOption = MergeOption.OverwriteChanges;
            ctx.AddObject("Suppliers", s);
            ctx.SaveChanges();

            var outdatedContext = new DataServiceContext(ctx.BaseUri);
            Supplier outdatedSupplier = outdatedContext.CreateQuery<Supplier>("Suppliers").Where(_s => _s.ID == s.ID).FirstOrDefault();
            
            s.Name = "something else";
            ctx.UpdateObject(s);
            ctx.SaveChanges();

            ctx.UpdateObject(s);
            ctx.SaveChanges();

            s = ctx.CreateQuery<Supplier>("Suppliers").Where(_s => _s.ID == s.ID).FirstOrDefault();
            Assert.AreEqual("something else", s.Name);

            outdatedContext.UpdateObject(outdatedSupplier);
            try
            {
                outdatedContext.SaveChanges();
                Assert.Fail("Concurrency failed to throw");
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("Concurrency: precondition failed for property 'Concurrency'"));
            }
        }

        [TestMethod]
        public void UpdateNullName()
        {
            Product p = CreateProduct();
            p.Name = null;
            ctx.AddObject("Products", p);

            ctx.SaveChanges();
            Assert.AreEqual(10, ctx.CreateQuery<Product>("Products").Count());
        }

        [TestMethod]
        public void UpdateAssociation_AddRefToCol()
        {
            ctx.MergeOption = MergeOption.OverwriteChanges;
            Category cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            Product p = ctx.CreateQuery<Product>("Products").Expand("Category, Category/Products").Skip(1).FirstOrDefault();
            Category oldCategory = p.Category;
            Assert.IsTrue(oldCategory.Products.Contains(p));

            // this should remove p from oldCategory
            ctx.AddLink(cate, "Products", p);
            ctx.SaveChanges();

            // refresh category and products
            ctx.CreateQuery<Category>("Categories").Expand("Products, Products/Category").ToList();
            Assert.IsFalse(oldCategory.Products.Contains(p));
            Assert.IsTrue(p.Category == cate);
            Assert.IsTrue(cate.Products.Contains(p));
            Assert.AreEqual(2, cate.Products.Count);
        }

        [TestMethod]
        public void UpdateAssociation_SetRef()
        {
            ctx.MergeOption = MergeOption.OverwriteChanges;
            Category cate = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            Product p = ctx.CreateQuery<Product>("Products").Expand("Category, Category/Products").Skip(1).FirstOrDefault();
            Category oldCategory = p.Category;
            Assert.IsTrue(oldCategory.Products.Contains(p));

            // this should remove p from oldCategory
            ctx.SetLink(p, "Category", cate);
            ctx.SaveChanges();

            // refresh category and products
            ctx.CreateQuery<Category>("Categories").Expand("Products, Products/Category").ToList();
            Assert.IsFalse(oldCategory.Products.Contains(p));
            Assert.IsTrue(p.Category == cate);
            Assert.IsTrue(cate.Products.Contains(p));
            Assert.AreEqual(2, cate.Products.Count);
        }

        [TestMethod]
        public void UpdateAssociation_SetLinkToNull()
        {
            ctx.MergeOption = MergeOption.OverwriteChanges;
            Product p = ctx.CreateQuery<Product>("Products").Expand("Category, Category/Products").Skip(1).FirstOrDefault();
            Category oldCategory = p.Category;
            Assert.IsTrue(oldCategory.Products.Contains(p));

            // this should remove p from oldCategory
            ctx.SetLink(p, "Category", null);
            ctx.SaveChanges();

            // refresh category and products
            ctx.CreateQuery<Product>("Products").Expand("Category, Category/Products").ToList();
            Assert.IsFalse(oldCategory.Products.Contains(p));
            Assert.IsTrue(p.Category == null);
        }

        [TestMethod]
        public void UpdateReadOnlyFailure()
        {
            Action trySaveChanges = new Action(() =>
            {
                try
                {
                    ctx.SaveChanges();
                    Assert.Fail("Save Changes failed to throw");
                }
                catch (DataServiceRequestException ex)
                {
                    DataServiceClientException innerEx = ex.InnerException as DataServiceClientException;
                    Assert.IsNotNull(innerEx);
                    Assert.AreEqual(403, innerEx.StatusCode);
                    Assert.IsTrue(innerEx.Message.Contains("You are connected to a read-only data session. Update operations are not permitted for your session"));
                }
                finally
                {
                    List<object> attachedEntities = new List<object>(
                        ctx.Entities.Select(e => e.Entity));
                    foreach (object entity in attachedEntities)
                    {
                        ctx.Detach(entity);
                    }
                }
            });

            UriBuilder builder = new UriBuilder(ctx.BaseUri);
            builder.Path = "/V2/OData/OData.svc/";
            ctx = new DataServiceContext(new Uri(builder.ToString()));

            Product p = CreateProduct();
            ctx.AddObject("Products", p);
            trySaveChanges();

            Supplier s = ctx.CreateQuery<Supplier>("Suppliers").FirstOrDefault();
            ctx.UpdateObject(s);
            trySaveChanges();

            Category c = ctx.CreateQuery<Category>("Categories").FirstOrDefault();
            ctx.DeleteObject(c);
            trySaveChanges();
        }

        // [TestMethod]
        public void UpdateOverloadSessionCache()
        {
            var oldContext = new DataServiceContext(ctx.BaseUri);
            oldContext.AddObject("Categories", CreateCategory());
            oldContext.SaveChanges();

            int count1 = oldContext.CreateQuery<Category>("Categories").Count();
            Assert.AreEqual(4, count1);

            for (int i = 0; i < 257; ++i)
            {
                CreateSession();
                Assert.AreEqual(3, ctx.CreateQuery<Category>("Categories").Count());
            }

            count1 = oldContext.CreateQuery<Category>("Categories").Count();
            Assert.AreEqual(3, count1);
        }

        private int GetNextId()
        {
            return nextId++;
        }

        private Category CreateCategory()
        {
            return new Category()
            {
                ID = GetNextId(),
                Name = "Fruits"
            };
        }

        private Supplier CreateSupplier()
        {
            return new Supplier()
            {
                ID = GetNextId(),
                Name = "New Supplier",
                Address = new Address()
                {
                    City = "City",
                    Country = "USA",
                    State = "ST",
                    Street = "Street",
                    ZipCode = "10000"
                }               
            };
        }

        private Product CreateProduct()
        {
            return new Product()
            {
                ID = GetNextId(),
                Name = "Apple",
                Description = "An apple a day keeps the doctors away",
                DiscontinuedDate = DateTime.Now,
                Rating = 4,
                Price = 1.0m
            };
        }

        private void CreateEntity(bool setLink)
        {
            Category newCate = CreateCategory();
            Product newProduct = CreateProduct();

            ctx.AddObject("Categories", newCate);
            ctx.AddObject("Products", newProduct);

            if (setLink)
            {
                ctx.SetLink(newProduct, "Category", newCate);
            }

            ctx.SaveChanges();
        }

        private void VerifyEntityCount(int productCount, int cateCount)
        {
            Assert.AreEqual(productCount, ctx.CreateQuery<Product>("Products").Count());
            Assert.AreEqual(cateCount, ctx.CreateQuery<Category>("Categories").Count());
        }

        private void VerifyContainerLimit(string entitySet, Func<Object> generator)
        {
            int i;
            for (i = 0; i < 100; ++i)
            {
                ctx.AddObject(entitySet, generator());
                try
                {
                    ctx.SaveChanges();
                }
                catch (DataServiceRequestException ex)
                {
                    Assert.IsTrue(ex.InnerException.Message.Contains("There are too many entities in the target set. Please start a new session and try again."));
                    break;
                }
            }

            Assert.AreEqual(entitySet == "Products" ? 42 : 48, i);
        }

    }
}
