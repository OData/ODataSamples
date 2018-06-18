// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using BasicEFCoreTest.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace BasicEFCoreTest.Controllers
{
    public class CustomersController : ODataController
    {
        private CustomerOrderContext _db;

        public CustomersController(CustomerOrderContext context)
        {
            _db = context;

            if (context.Database.EnsureCreated())
            {
                if (context.Customers.Count() == 0)
                {
                    foreach(var customer in DataSource.GetCustomers())
                    {
                        context.Customers.Add(customer);
                        context.Orders.Add(customer.Order);
                    }

                    context.SaveChanges();
                }
            }
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_db.Customers);
        }
    }
}
