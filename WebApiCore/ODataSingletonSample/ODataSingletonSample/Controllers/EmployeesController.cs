// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace ODataSingletonSample
{
    public class EmployeesController : ODataController
    {
        public static List<Employee> Employees;

        static EmployeesController()
        {
            InitData();
        }

        private static void InitData()
        {
            Employees = Enumerable.Range(0, 10).Select(i =>
                   new Employee()
                   {
                       ID = i,
                       Name = string.Format("Name {0}", i)
                   }).ToList();
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(Employees.AsQueryable());
        }

        public IActionResult Get(int key)
        {
            return Ok(Employees.Where(e => e.ID == key));
        }

        [EnableQuery]
        public IActionResult GetEmployees()
        {
            return Ok(Employees.AsQueryable());
        }

        public IActionResult GetCompanyFromEmployee([FromODataUri] int key)
        {
            var company = Employees.First(e => e.ID == key).Company;
            if (company == null)
            {
                return StatusCode(404); // HttpStatusCode.NotFound
            }
            return Ok(company);
        }

        public IActionResult POST([FromBody] Employee employee)
        {
            Employees.Add(employee);
            return Created(employee);
        }

        [AcceptVerbs("PUT")]
        public IActionResult CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        {
            if (navigationProperty != "Company")
            {
                return BadRequest();
            }

            Employees.First(e => e.ID == key).Company = UmbrellaController.Umbrella;
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        public IActionResult DeleteRef([FromODataUri] int key, string navigationProperty)
        {
            if (navigationProperty != "Company")
            {
                return BadRequest();
            }

            Employees.First(e => e.ID == key).Company = null;
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        public IActionResult PutToCompany(int key, Company company)
        {
            var navigateCompany = Employees.First(e => e.ID == key).Company;
            Employees.First(e => e.ID == key).Company = company;
            if (navigateCompany.Name == "Umbrella")
            {
                UmbrellaController.Umbrella = navigateCompany;
            }
            else
            {
                return BadRequest();
            }
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        public IActionResult PatchToCompany(int key, Delta<Company> company)
        {
            var navigateCompany = Employees.First(e => e.ID == key).Company;
            company.Patch(Employees.First(e => e.ID == key).Company);
            if (navigateCompany.Name == "Umbrella")
            {
                company.Patch(UmbrellaController.Umbrella);
            }
            else
            {
                return BadRequest();
            }
            return StatusCode(204); // HttpStatusCode.NoContent
        }

        [HttpPost]
        public IActionResult ResetDataSourceOnCollectionOfEmployee()
        {
            InitData();
            return Ok();
        }
    }
}
