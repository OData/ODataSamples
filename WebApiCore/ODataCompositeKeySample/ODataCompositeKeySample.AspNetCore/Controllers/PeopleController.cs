// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataCompositeKeySample.Models;

namespace ODataCompositeKeySample.AspNet.Controllers
{
    public class PeopleController : ODataController
    {
        private PeopleRepository _repo = new PeopleRepository();

        [HttpGet]
        [EnableQuery]
        public IEnumerable<Person> Get()
        {
            return _repo.Get();
        }

        [EnableQuery]
        public IActionResult Get([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName)
        {
            Person person = _repo.Get(keyFirstName, keyLastName);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        public IActionResult PutPerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName, Person person)
        {
            _repo.UpdateOrAdd(person);

            return Updated(person);
        }

        [HttpPatch]
        public IActionResult PatchPerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName, Delta<Person> delta)
        {
            var person = _repo.Get(keyFirstName, keyLastName);
            if (person == null)
            {
                return NotFound();
            }

            delta.Patch(person);

            person.FirstName = keyFirstName;
            person.LastName = keyLastName;
            _repo.UpdateOrAdd(person);

            return Updated(person);
        }

        public IActionResult PostPerson([FromBody] Person person)
        {
            _repo.UpdateOrAdd(person);
            return Created(person);
        }

        public IActionResult DeletePerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName)
        {
            var person = _repo.Remove(keyFirstName, keyLastName);
            if (person == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
