// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataBatchSample.Models;

namespace ODataBatchSample.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly CustomersContext context;

        public CustomersController(CustomersContext context)
        {
            this.context = context;
        }

        [EnableQuery(PageSize = 10, MaxExpansionDepth = 2)]
        public IActionResult Get()
        {
            return Ok(context.Customers);
        }

        public async Task<IActionResult> Post([FromBody] Customer entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Customers.Add(entity);
            await context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Customer entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (key != entity.Id)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            var originalCustomer = await context.Customers.FindAsync(key);
            if (originalCustomer == null)
            {
                return NotFound();
            }
            else
            {
                context.Entry(originalCustomer).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync();
            }
            return Updated(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<Customer> patch)
        {
            object id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (patch.GetChangedPropertyNames().Contains("Id") && patch.TryGetPropertyValue("Id", out id) && (int)id != key)
            {
                return BadRequest("The key from the url must match the key of the entity in the body");
            }
            Customer originalEntity = await context.Customers.FindAsync(key);
            if (originalEntity == null)
            {
                return NotFound();
            }
            else
            {
                patch.Patch(originalEntity);
                await context.SaveChangesAsync();
            }
            return Updated(originalEntity);
        }


        public async Task<IActionResult> Delete([FromODataUri]int key)
        {
            Customer entity = await context.Customers.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                context.Customers.Remove(entity);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}