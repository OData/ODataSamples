// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BasicWebApiSample.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace BasicWebApiSample.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly CustomerOrderContext _context;

        public CustomersController(CustomerOrderContext context)
        {
            _context = context;
            Generate(_context);
        }

        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            if (Request.Path.Value.Contains("inmem"))
            {
                return GetCustomers().AsQueryable();
            }
            else
            {
                return _context.Customers;
            }
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            if (Request.Path.Value.Contains("inmem"))
            {
                return Ok(GetCustomers().FirstOrDefault(c => c.Id == key));
            }
            else
            {
                return Ok(_context.Customers.FirstOrDefault(c => c.Id == key));
            }
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Created(customer);
        }

        private static IList<Customer> _customers;
        public static IList<Customer> GetCustomers()
        {
            if (_customers != null)
            {
                return _customers;
            }
            Customer customerA = new Customer
            {
                Id = 1,
                Name = "Customer A",
                Birthday = new DateTime(1990, 1, 1),
                EffectiveDate = new Date(2001, 8, 8),
                FavoriateColor = Color.Red,
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Street = "156 AVE NE"
                },
                Order = new Order
                {
                    Id = 101,
                    Price = 101m
                },
                Orders = Enumerable.Range(1, 3).Select(e => new Order
                {
                    Id = 10 + e,
                    Price = 10.8m * e
                }).ToList()
            };

            Customer customerB = new Customer
            {
                Id = 2,
                Name = "Customer B",
                Birthday = new DateTime(2009, 11,9),
                EffectiveDate = new Date(2011, 10, 18),
                FavoriateColor = Color.Red,
                HomeAddress = new Address
                {
                    City = "Bellevue",
                    Street = "Main St NE"
                },
                Order = new Order
                {
                    Id = 102,
                    Price = 104m
                },
                Orders = Enumerable.Range(1, 4).Select(e => new Order
                {
                    Id = 20 + e,
                    Price = 28.8m / e
                }).ToList()
            };

            _customers = new List<Customer>
            {
                customerA,
                customerB
            };

            return _customers;
        }

        public static void Generate(CustomerOrderContext context)
        {
            if (context.Customers.Any())
            {
                return;
            }

            var customers = GetCustomers();

            foreach (var c in customers)
            {
                foreach (var o in c.Orders)
                {
                    context.Orders.Add(o);
                }

                context.Customers.Add(c);
            }
            context.SaveChanges();
        }
    }
}
