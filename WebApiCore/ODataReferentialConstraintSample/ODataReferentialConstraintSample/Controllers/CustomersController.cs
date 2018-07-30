// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataReferentialConstraintSample.Models;

namespace ODataReferentialConstraintSample
{
    public class CustomersController : ODataController
    {
        private CustomerOrderReferentialConstraintContext _db;

        public CustomersController(CustomerOrderReferentialConstraintContext context)
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
            Customer customer = _db.Customers.FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        public IActionResult Delete(int key)
        {
            Customer customer = _db.Customers.FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            _db.Customers.Remove(customer);
            _db.SaveChanges();

            return StatusCode(204); // HttpStatusCode.NoContent
        }

        [HttpPost]
        [ODataRoute("ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            if (_db.Database.EnsureDeleted())
            {
                if (_db.Customers.Count() == 0)
                {
                    foreach (var customer in DataSource.Customers)
                    {
                        _db.Customers.Add(customer);
                        foreach (var order in customer.Orders)
                        {
                            _db.Orders.Add(order);
                        }
                    }

                    _db.SaveChanges();
                }
            }

            return Ok();
        }
    }
}
