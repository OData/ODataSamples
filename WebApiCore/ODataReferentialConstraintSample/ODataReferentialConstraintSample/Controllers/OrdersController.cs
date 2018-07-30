// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataReferentialConstraintSample.Models;

namespace ODataReferentialConstraintSample
{
    public class OrdersController : ODataController
    {
        private CustomerOrderReferentialConstraintContext _db;

        public OrdersController(CustomerOrderReferentialConstraintContext context)
        {
            _db = context;

            if (context.Database.EnsureCreated())
            {
                if (context.Customers.Count() == 0)
                {
                    foreach (var customer in DataSource.Customers)
                    {
                        context.Customers.Add(customer);
                        foreach (var order in customer.Orders)
                        {
                            context.Orders.Add(order);
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            Order order = _db.Orders.FirstOrDefault(c => c.OrderId == key);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
