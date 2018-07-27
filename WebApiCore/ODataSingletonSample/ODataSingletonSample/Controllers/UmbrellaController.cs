// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ODataSingletonSample
{
    /// <summary>
    /// Present a singleton named "Umbrella"
    /// Use convention routing
    /// </summary>
    public class UmbrellaController : ODataController
    {
        public static Company Umbrella;

        static UmbrellaController()
        {
            InitData();
        }

        private static void InitData()
        {
            Umbrella = new Company()
            {
                ID = 1,
                Name = "Umbrella",
                Revenue = 1000,
                Category = CompanyCategory.Communication,
                Employees = new List<Employee>()
            };
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(Umbrella);
        }

        public IActionResult GetRevenueFromCompany()
        {
            return Ok(Umbrella.Revenue);
        }

        public IActionResult GetName()
        {
            return Ok(Umbrella.Name);
        }

        public IActionResult GetEmployeesFromCompany()
        {
            return Ok(Umbrella.Employees);
        }

        public IActionResult Put(Company newCompany)
        {
            Umbrella = newCompany;
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        public IActionResult Patch(Delta<Company> item)
        {
            item.Patch(Umbrella);
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        [AcceptVerbs("POST")]
        public IActionResult CreateRef(string navigationProperty, [FromBody] Uri link)
        {
            int relatedKey = HelperFunction.GetKeyValue<int>(link);
            Employee employee = EmployeesController.Employees.First(x => x.ID == relatedKey);

            if (navigationProperty != "Employees" || employee == null)
            {
                return BadRequest();
            }

            if (Umbrella.Employees == null)
            {
                Umbrella.Employees = new List<Employee>() {employee};
            }
            else
            {
                Umbrella.Employees.Add(employee);
            }

            return StatusCode(204); // HttpStatusCode.NoContent
        }

        [AcceptVerbs("DELETE")]
        public IActionResult DeleteRef(string relatedKey, string navigationProperty)
        {
            int key = int.Parse(relatedKey);
            Employee employee = Umbrella.Employees.First(x => x.ID == key);

            if (navigationProperty != "Employees")
            {
                return BadRequest();
            }

            Umbrella.Employees.Remove(employee);
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        [HttpPost]
        public IActionResult PostToEmployees([FromBody] Employee employee)
        {
            EmployeesController.Employees.Add(employee);
            if (Umbrella.Employees == null)
            {
                Umbrella.Employees = new List<Employee>() { employee };
            }
            else
            {
                Umbrella.Employees.Add(employee);
            }

            return Created(employee);
        }

        [HttpPost]
        [ODataRoute("Umbrella/ODataSingletonSample.ResetDataSource")]
        public IActionResult ResetDataSourceOnCompany()
        {
            InitData();
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        public IActionResult GetEmployeesCount()
        {
            return Ok(Umbrella.Employees.Count);
        }
    }
}
