// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV3Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Data.Services.Common;
    using System.Linq;
    using System.Spatial;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataWebV3Tests.ODataDemoV3Model;

    [TestClass]
    public class ODataWebSessionTest
    {
        private DemoService ctx;
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
            Uri sessionedUri = new Uri("http://localhost:32026/V3/(S(" + sessionId + "))/OData/OData.svc/", UriKind.Absolute);
#else
            Uri sessionedUri = new Uri("http://services.odata.org/V3/(S(" + sessionId + "))/OData/OData.svc/", UriKind.Absolute);
#endif
            ctx = new DemoService(sessionedUri);
            AddTypeResolvers(ctx);
        }

        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void EntityCountAcrossSession()
        {
            VerifyEntityCount(11, 3);
            CreateEntity(false);

            CreateSession();
            VerifyEntityCount(11, 3);
        }

        [TestMethod]
        public void EntityCountSameSession()
        {
            VerifyEntityCount(11, 3);
            CreateEntity(true);
            VerifyEntityCount(12, 4);
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

            var outdatedContext = new DataServiceContext(ctx.BaseUri, DataServiceProtocolVersion.V3);
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
            Assert.AreEqual(12, ctx.CreateQuery<Product>("Products").Count());
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
            builder.Path = "/V3/OData/OData.svc/";
            ctx = new DemoService(new Uri(builder.ToString()));
            AddTypeResolvers(ctx);

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

        [TestMethod]
        public void CallActionTest()
        {
            ctx.Execute(new Uri("Products(1)/Discount", UriKind.Relative), "POST", new BodyOperationParameter("discountPercentage", 10));
            Product product = ctx.CreateQuery<Product>("Products").Where(p => p.ID == 1).FirstOrDefault();
            Assert.AreEqual((decimal)3.15, product.Price);
        }

        [TestMethod]
        public void SpatialPropertyTest()
        {
            Supplier s = ctx.CreateQuery<Supplier>("Suppliers").FirstOrDefault();
            Assert.AreEqual(GeographyPoint.Create(47.6316604614258, -122.03547668457), s.Location);
        }

        [TestMethod]
        public void SpatialPropertyOrderByDistanceTest()
        {
            var s = ctx.Execute<Supplier>(new Uri("Suppliers?$orderby=geo.distance(Location,geography'SRID=4326;POINT(47.515986 22.405)')", UriKind.Relative)).ToArray();
            Assert.AreEqual(2, s.Count());
            Assert.AreEqual(1, s[0].ID);
            Assert.AreEqual(0, s[1].ID);
        }

        [TestMethod]
        public void PatchTest()
        {
            Supplier s = ctx.CreateQuery<Supplier>("Suppliers").FirstOrDefault();
            ctx.UpdateObject(s);
            ctx.SaveChanges(SaveChangesOptions.PatchOnUpdate);
        }

        [TestMethod]
        public void PreferHeaderTest()
        {
            ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
            Supplier s = ctx.CreateQuery<Supplier>("Suppliers").FirstOrDefault();
            ctx.UpdateObject(s);
            ctx.SaveChanges();
        }

        [TestMethod]
        public void TypeCastTest()
        {
            Product product = ctx.Execute<Product>(new Uri("Products/ODataDemo.Product", UriKind.Relative)).FirstOrDefault();
            Assert.AreEqual(0, product.ID);
        }

        [TestMethod]
        public void TestBatch()
        {
            Product product = ctx.CreateQuery<Product>("Products").FirstOrDefault();
            ctx.UpdateObject(product);
            ctx.SaveChanges(SaveChangesOptions.Batch);
        }

        [TestMethod]
        public void TestBatchWithManualUri()
        {
            Product product = ctx.CreateQuery<Product>("Products").FirstOrDefault();
            DataServiceContext ctxNew = new DataServiceContext(ctx.BaseUri, DataServiceProtocolVersion.V3);
            ctxNew.AttachTo("Products", product);
            ctxNew.UpdateObject(product);
            AddTypeResolvers(ctxNew);
            ctxNew.SaveChanges(SaveChangesOptions.Batch);
        }

        [TestMethod]
        public void TestDerivedTypeQuery()
        {
            var featuredProducts = ctx.CreateQuery<Product>("Products").OfType<FeaturedProduct>().ToArray();
            Assert.AreEqual(2, featuredProducts.Count());
        }

        [TestMethod]
        public void TestJsonInsert()
        {
            ctx.Format.UseJson();
            CreateEntity(true);
            ctx.Format.UseAtom();
        }

        [TestMethod]
        public void TestJsonUpdate()
        {
            ctx.Format.UseJson();
            var product = ctx.CreateQuery<Product>("Products").First();
            product.Description = "New description";
            ctx.SaveChanges();
            ctx.Format.UseAtom();
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
                ReleaseDate = DateTime.Now.AddYears(-1),
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

            Assert.AreEqual(entitySet == "Products" ? 40 : 48, i);
        }

        private void AddTypeResolvers(DataServiceContext ctx)
        {
            ctx.ResolveName = (entityType) =>
                {
                    return entityType.FullName.Replace("ODataWebV3Tests.ODataDemoV3Model", "ODataDemo");
                };

            ctx.ResolveType = (name) =>
                {
                    string clientName = name.Replace("ODataDemo", "ODataWebV3Tests.ODataDemoV3Model"); 
                    return Type.GetType(clientName);
                };
        }
    }
}
