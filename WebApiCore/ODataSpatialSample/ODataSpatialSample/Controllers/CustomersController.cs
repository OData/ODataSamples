// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataSpatialSample.Models;

namespace ODataSpatialSample.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly SpatialDataContext _db;


        public CustomersController(SpatialDataContext db)
        {
            _db = db;
            BuildDatabase(db);
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_db.Customers);
        }

        public IActionResult Get(int key)
        {
            Customer customer = _db.Customers.FirstOrDefault(e => e.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        public IActionResult Post([FromBody]Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Post the spatial type doesn't support yet!");
            }

            _db.Customers.Add(customer);
            _db.SaveChanges();
            return Created(customer);
        }

        private static void BuildDatabase(SpatialDataContext db)
        {
            if (db.Customers.Any())
            {
                return;
            }

            string[] names = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

            var customers = Enumerable.Range(1, 7).Select(e => new Customer
            {
                Id = e,
                Name = names[e - 1],
                DbLocation = String.Format("POINT({0} {1} {2} {3})", e, e, e, e)
            });

            foreach (var customer in customers)
            {
                db.Customers.Add(customer);
            }

            db.SaveChanges();
        }
    }
}
