// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODataEtagSample.Models;

namespace ODataEtagSample.Controllers
{
    public class CustomersController : ODataController
    {
        private CustomersContext _context;

        public CustomersController(CustomersContext context)
        {
            _context = context;

            if (context.Database.EnsureCreated())
            {
                if (context.Customers.Count() == 0)
                {
                    var customers = Enumerable.Range(1, 10).Select(i => new Customer
                    {
                        Id = i,
                        Age = 18 + i,
                        Name = "Customer " + i,
                    });

                    foreach (var customer in customers)
                    {
                        context.Customers.Add(customer);
                    }

                    context.SaveChanges();
                }
            }
        }

        [EnableQuery]
        public IActionResult Get(int key, ODataQueryOptions<Customer> options)
        {
            IQueryable<Customer> customerByIdQuery = _context.Customers.Where(c => c.Id == key);
            if (options.IfNoneMatch != null)
            {
                IQueryable<Customer> customerQuery = options.IfNoneMatch.ApplyTo(customerByIdQuery) as IQueryable<Customer>;
                if (!customerQuery.Any())
                {
                    // The entity has the same ETag as the one in the If-None-Match header of the request,
                    // so it hasn't been modified since it was retrieved the first time.
                    return StatusCode(304); // HttpStatusCode.NotModified
                }
                else
                {
                    // The entity has a different ETag than the one specified in the If-None-Match header of the request,
                    // so we return the entity.
                    return Ok(SingleResult<Customer>.Create(customerByIdQuery));
                }
            }
            else
            {
                // The request didn't contain any ETag, so we return the entity.
                return Ok(SingleResult<Customer>.Create(customerByIdQuery));
            }
        }

        public async Task<IActionResult> Put(int key, Customer customer, ODataQueryOptions<Customer> options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(key == customer.Id))
            {
                return BadRequest("The customer Id must match the key in the URI");
            }
            if (options.IfMatch != null)
            {
                if (!_context.Customers.Where(c => c.Id == key).Any())
                {
                    // The entity doesn't exist on the database and as the request contains an If-Match header we don't
                    // insert the entity instead (No UPSERT behavior if the If-Match header is present).
                    return NotFound();
                }
                else if (!((IQueryable<Customer>)options.IfMatch.ApplyTo(_context.Customers.Where(c => c.Id == key))).Any())
                {
                    // The ETag of the entity doesn't match the value sent on the If-Match header, so the entity has
                    // been modified by a third party between the entity retrieval and update..
                    return StatusCode(412); // HttpStatusCode.PreconditionFailed
                }
                else
                {
                    // The entity exists in the database and the ETag of the entity matches the value on the If-Match 
                    // header, so we update the entity.
                    _context.Entry(customer).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok(customer);
                }
            }
            else
            {
                if (!_context.Customers.Where(c => c.Id == key).Any())
                {
                    // The request didn't contain any If-Match header and the entity doesn't exist on the database, so
                    // we create a new one. For more details see the section 11.4.4 of the OData v4.0 specification.
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    return base.Created(customer);
                }
                else
                {
                    // the request didn't contain any If-Match header and the entity exists on the database, so we
                    // update it's value.
                    _context.Entry(customer).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok(customer);
                }
            }
        }
    }
}
