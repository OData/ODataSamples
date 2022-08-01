﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Lab01Sample01.Models;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Deltas;

namespace Lab01Sample01.Controllers
{
    public class PublishersController : ODataController
    {
        #region CRUD operations
        // Get ~/Publishers
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(DataSource.Instance.Publishers);
        }

        // GET ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // POST ~/Publishers
        [EnableQuery]
        public IActionResult Post([FromBody] Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DataSource.Instance.Publishers.Add(publisher);

            return Created(publisher);
        }

        // PUT ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Publisher Publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // PATCH ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Publisher> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // DELETE ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);
            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }
        #endregion
    }
}
