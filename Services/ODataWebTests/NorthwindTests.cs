// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Services.Client;
using System.Collections;

namespace ODataWebTests
{
    [TestClass]
    public class NorthwindTests
    {
        private NorthwindClientModel.NorthwindEntities context;

        [TestInitialize()]
        public void TestInitialize()
        {
#if DEBUG
            this.context = new NorthwindClientModel.NorthwindEntities(new Uri("http://localhost:51514/V2/Northwind/Northwind.svc/", UriKind.Absolute));
#else
            this.context = new NorthwindClientModel.NorthwindEntities(new Uri("http://services.odata.org/V2/Northwind/Northwind.svc/", UriKind.Absolute));
#endif
        }

        [TestMethod]
        public void QueryEntitySets()
        {
            Assert.AreEqual(20, context.CreateQuery<NorthwindClientModel.Customer>("Customers").ToList().Count());
            Assert.AreEqual(8, context.CreateQuery<NorthwindClientModel.Category>("Categories").ToList().Count());
            Assert.AreEqual(9, context.CreateQuery<NorthwindClientModel.Employee>("Employees").ToList().Count());
            Assert.AreEqual(500, context.CreateQuery<NorthwindClientModel.Order_Detail>("Order_Details").ToList().Count());
            Assert.AreEqual(200, context.CreateQuery<NorthwindClientModel.Order>("Orders").ToList().Count());
            Assert.AreEqual(20, context.CreateQuery<NorthwindClientModel.Product>("Products").ToList().Count());
            Assert.AreEqual(4, context.CreateQuery<NorthwindClientModel.Region>("Regions").ToList().Count());
            Assert.AreEqual(3, context.CreateQuery<NorthwindClientModel.Shipper>("Shippers").ToList().Count());
            Assert.AreEqual(29, context.CreateQuery<NorthwindClientModel.Supplier>("Suppliers").ToList().Count());
            Assert.AreEqual(53, context.CreateQuery<NorthwindClientModel.Territory>("Territories").ToList().Count());
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
            var cust = NorthwindClientModel.Customer.CreateCustomer("ALFKI2", "MS");
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
            var q = context.CreateQuery<NorthwindClientModel.Customer>("Customers").Expand("Orders");

            int totalCustomerCount = q.Count();
            int totalOrdersCount = context.CreateQuery<NorthwindClientModel.Order>("Orders").Count();

            var qor = q.Execute() as QueryOperationResponse<NorthwindClientModel.Customer>;

            DataServiceQueryContinuation<NorthwindClientModel.Customer> nextCustLink = null;
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
                            var innerQOR = context.Execute<NorthwindClientModel.Order>(nextOrderLink) as QueryOperationResponse<NorthwindClientModel.Order>;
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
                    qor = context.Execute<NorthwindClientModel.Customer>(nextCustLink) as QueryOperationResponse<NorthwindClientModel.Customer>;
                }

            } while (nextCustLink != null);

            Assert.AreEqual(totalCustomerCount, custCount);
            Assert.AreEqual(totalOrdersCount, orderCount);
            Assert.AreEqual(totalOrdersCount, context.Links.Count);
            Assert.AreEqual(totalCustomerCount + totalOrdersCount, context.Entities.Count);
        }
    }
}
