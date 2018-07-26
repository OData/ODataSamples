// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

#if NETCOREAPP2_1
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
#endif

namespace ODataEnumTypeSample
{
    public class EmployeesController : ODataController
    {
        /// <summary>
        /// static so that the data is shared among requests.
        /// </summary>
        private static IList<Employee> _employees = null;

        public EmployeesController()
        {
            if (_employees == null)
            {
                InitEmployees();
            }
        }

        [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
#if NETCOREAPP2_1
        public IActionResult
#else
        public IHttpActionResult
#endif
            Get()
        {
            return Ok(_employees.AsQueryable());
        }

#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            Get(int key)
        {
            return Ok(_employees.Single(e => e.ID == key));
        }

#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            GetAccessLevelFromEmployee(int key)
        {
            return Ok(_employees.Single(e => e.ID == key).AccessLevel);
        }

#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            GetNameFromEmployee(int key)
        {
            return Ok(_employees.Single(e => e.ID == key).Name);
        }

#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            Post(Employee employee)
        {
            employee.ID = _employees.Count + 1;
            _employees.Add(employee);

            return Created(employee);
        }

        [HttpPost]
        [ODataRoute("Employees({key})/ODataEnumTypeSample.AddAccessLevel")]
#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            AddAccessLevel(int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            String accessLevelOfString = (String)parameters["AccessLevel"];
            AccessLevel accessLevelOfAccesslevel = (AccessLevel)Enum.Parse(typeof(AccessLevel), accessLevelOfString);

            Employee employee = _employees.Single(e => e.ID == key);
            if (null == employee)
            {
                return BadRequest();
            }

            employee.AccessLevel |= accessLevelOfAccesslevel;

            return Ok(employee.AccessLevel);
        }

        [HttpGet]
        [ODataRoute("HasAccessLevel(ID={id},AccessLevel={accessLevel})")]
#if NETCOREAPP2_1
        public IActionResult 
#else
        public IHttpActionResult 
#endif
            HasAccessLevel([FromODataUri] int id, [FromODataUri] AccessLevel accessLevel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Employee employee = _employees.Single(e => e.ID == id);
            var result = employee.AccessLevel.HasFlag(accessLevel);
            return Ok(result);
        }

        private void InitEmployees()
        {
            _employees = new List<Employee>
            {
                new Employee()
                {
                    ID = 1,
                    Name = "Lisa",
                    Gender = Gender.Female,
                    AccessLevel = AccessLevel.Execute,
                },
                new Employee()
                {
                    ID = 2,
                    Name = "Bob",
                    Gender = Gender.Male,
                    AccessLevel = AccessLevel.Read,
                },
                new Employee(){
                    ID = 3,
                    Name = "Alice",
                    Gender = Gender.Female,
                    AccessLevel = AccessLevel.Read | AccessLevel.Write,
                },
            };
        }
    }
}
