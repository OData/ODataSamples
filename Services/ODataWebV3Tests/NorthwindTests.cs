// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV3Tests
{
    using System;
    using System.Collections;
    using System.Data.Services.Client;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataWebV3Tests.NorthwindV3ClientModel;

    [TestClass]
    public class NorthwindTests
    {
        private NorthwindEntities context;

        [TestInitialize()]
        public void TestInitialize()
        {
#if DEBUG
            this.context = new NorthwindEntities(new Uri("http://localhost:32026/V3/Northwind/Northwind.svc/", UriKind.Absolute));
#else
            this.context = new NorthwindEntities(new Uri("http://services.odata.org/V3/Northwind/Northwind.svc/", UriKind.Absolute));
#endif
        }

        [TestMethod]
        public void QueryEntitySets()
        {
            Assert.AreEqual(20, context.CreateQuery<Customer>("Customers").ToList().Count());
            Assert.AreEqual(8, context.CreateQuery<Category>("Categories").ToList().Count());
            Assert.AreEqual(9, context.CreateQuery<Employee>("Employees").ToList().Count());
            Assert.AreEqual(500, context.CreateQuery<Order_Detail>("Order_Details").ToList().Count());
            Assert.AreEqual(200, context.CreateQuery<Order>("Orders").ToList().Count());
            Assert.AreEqual(20, context.CreateQuery<Product>("Products").ToList().Count());
            Assert.AreEqual(4, context.CreateQuery<Region>("Regions").ToList().Count());
            Assert.AreEqual(3, context.CreateQuery<Shipper>("Shippers").ToList().Count());
            Assert.AreEqual(29, context.CreateQuery<Supplier>("Suppliers").ToList().Count());
            Assert.AreEqual(53, context.CreateQuery<Territory>("Territories").ToList().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DataServiceRequestException))]
        public void UpdateFailure()
        {
            var cust = context.Customers.FirstOrDefault();

            context.UpdateObject(cust);
            context.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(DataServiceRequestException))]
        public void UpdateFailure2()
        {
            var cust = Customer.CreateCustomer("ALFKI2", "MS");
            context.AddObject("Customers", cust);
            context.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(DataServiceRequestException))]
        public void UpdateFailure3()
        {
            var cust = context.Customers.FirstOrDefault();

            context.DeleteObject(cust);
            context.SaveChanges();
        }
           
        [TestMethod]
        public void QueryAllCustomersWithOrders()
        {
            var q = context.CreateQuery<Customer>("Customers").Expand("Orders");

            int totalCustomerCount = q.Count();
            int totalOrdersCount = context.CreateQuery<Order>("Orders").Count();

            var qor = q.Execute() as QueryOperationResponse<Customer>;

            DataServiceQueryContinuation<Customer> nextCustLink = null;
            int custCount = 0;
            int orderCount = 0;
            do
            {
                ICollection previousOrderCollection = null;

                foreach (var c in qor)
                {
                    try
                    {
                        if (previousOrderCollection != null)
                        {
                            qor.GetContinuation(previousOrderCollection);
                            Assert.Fail("Out of scope collection did not throw");
                        }
                    }
                    catch (ArgumentException)
                    {
                    }

                    var nextOrderLink = qor.GetContinuation(c.Orders);
                    while (nextOrderLink != null)
                    {
                        if (custCount % 2 == 0)
                        {
                            var innerQOR = context.Execute<Order>(nextOrderLink) as QueryOperationResponse<Order>;
                            foreach (var innerOrder in innerQOR)
                            {
                                context.AttachLink(c, "Orders", innerOrder);
                                c.Orders.Add(innerOrder);
                            }
                            nextOrderLink = innerQOR.GetContinuation();
                        }
                        else
                        {
                            nextOrderLink = context.LoadProperty(c, "Orders", nextOrderLink).GetContinuation();
                        }
                    }

                    previousOrderCollection = c.Orders;

                    orderCount += c.Orders.Count;
                    custCount++;
                }

                nextCustLink = qor.GetContinuation();
                if (nextCustLink != null)
                {
                    qor = context.Execute<Customer>(nextCustLink) as QueryOperationResponse<Customer>;
                }

            } while (nextCustLink != null);

            Assert.AreEqual(totalCustomerCount, custCount);
            Assert.AreEqual(totalOrdersCount, orderCount);
            Assert.AreEqual(totalOrdersCount, context.Links.Count);
            Assert.AreEqual(totalCustomerCount + totalOrdersCount, context.Entities.Count);
        }
    }
}
