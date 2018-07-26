// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.OData;
using ODataCompositeKeySample.AspNet.Extensions;
using ODataCompositeKeySample.Models;

namespace ODataCompositeKeySample.AspNet.Controllers
{
    [ModelValidationFilter]
    public class PeopleController : ODataController
    {
        private PeopleRepository _repo = new PeopleRepository();

        [EnableQuery]
        public IEnumerable<Person> Get()
        {
            return _repo.Get();
        }

        [EnableQuery]
        public Person Get([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName)
        {
            Person person = _repo.Get(keyFirstName, keyLastName);
            if (person == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return person;
        }

        public IHttpActionResult PutPerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName, Person person)
        {
            _repo.UpdateOrAdd(person);

            return Updated(person);
        }

        [AcceptVerbs("PATCH")]
        public IHttpActionResult PatchPerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName, Delta<Person> delta)
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

        public IHttpActionResult PostPerson(Person person)
        {
            _repo.UpdateOrAdd(person);
            return Created(person);
        }

        public IHttpActionResult DeletePerson([FromODataUri] string keyFirstName, [FromODataUri] string keyLastName)
        {
            var person = _repo.Remove(keyFirstName, keyLastName);
            if (person == null)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
