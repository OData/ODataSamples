// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.OData.Client;

namespace ODataSingletonSample.Client
{
    class Program
    {
        private static readonly string _baseAddress = "http://localhost:2629/odata";

        static void Main(string[] args)
        {
            ExecuteAsync(args);
        }

        /// <summary>
        /// Singleton Sample Code
        /// Umbrella is a singleton, Employees is an entityset
        /// Umbrella has a navigation property "Employees", which is binded to the Employees entityset
        /// Employees has a navigation property "Company", which is binded to Umbrella
        /// </summary>
        /// <param name="args"></param>
        static async void ExecuteAsync(string[] args)
        {
            // Core version can't work now. Will dig more.

            Container clientContext = new Container(new Uri(_baseAddress));
            clientContext.Format.UseJson();
            clientContext.MergeOption = MergeOption.OverwriteChanges;

            // Query Singleton
            Console.WriteLine("Query Singleton... ");
            var company = await clientContext.Umbrella.GetValueAsync();

            // Call bound action to reset singleton
            company.ResetDataSource();

            // Call bound action to reset entityset
            await clientContext.ExecuteAsync(new Uri(clientContext.BaseUri + "/Employees/ODataSingletonSample.ResetDataSource"), "POST");
            company = await clientContext.Umbrella.GetValueAsync();
            Console.WriteLine("Company name is: " + company.Name);
            Console.WriteLine("Company revenue is: " + company.Revenue);

            // Update singleton by PATCH
            company.Name = "Umbrella-NewName";
            clientContext.UpdateObject(company);
            await clientContext.SaveChangesAsync();

            // Query singleton property
            var nameAsync = await clientContext.ExecuteAsync<string>(new Uri("Umbrella/Name", UriKind.Relative));
            var name = nameAsync.Single();
            Console.WriteLine("After update Company name by PATCH, Company name is: " + name);

            // Update singleton by PUT
            company.Revenue = 1200;
            clientContext.UpdateObject(company);
            await clientContext.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

            // Query singleton property
            var revenue = (await clientContext.ExecuteAsync<Int64>(new Uri("Umbrella/Revenue", UriKind.Relative))).Single();
            Console.WriteLine("After update Company revenue by PUT, Company revenue is: " + revenue);

            // Add navigation link by creating a new entity
            Employee newEmployee1 = new Employee() { ID = 1111, Name = "NewHired1" };
            clientContext.AddRelatedObject(company, "Employees", newEmployee1);
            await clientContext.SaveChangesAsync();

            // Add navigation link based on existing entity
            Employee newEmployee2 = new Employee() { ID = 2222, Name = "NewHired2" };

            clientContext.AddToEmployees(newEmployee2);
            await clientContext.SaveChangesAsync();
            clientContext.AddLink(company, "Employees", newEmployee2);
            await clientContext.SaveChangesAsync();

            // Call unbound function
            var employeesCount = company.GetEmployeesCount();
            Console.WriteLine("After adding employees to Company, Company employees count is: " + employeesCount);

            // Load property
            Console.WriteLine("Company Employees names are: ");
            await clientContext.LoadPropertyAsync(company, "Employees");
            var employees = company.Employees;
            foreach (var emp in employees)
            {
                Console.WriteLine(emp.Name);
            }

            // Query Option - $select
            Console.WriteLine("Execute: GET http://localhost:50268/odata/Umbrella?$select=Name,Revenue");
            await clientContext.Umbrella.Select(c => new Company { Name = c.Name, Revenue = c.Revenue }).GetValueAsync();

            // Query Option - $expand
            Console.WriteLine("Execute: GET http://localhost:50268/odata/Umbrella?$expand=Employees");
            await clientContext.Umbrella.Expand(c => c.Employees).GetValueAsync();

            // Delete navigation link
            Console.WriteLine("Delete navigation link: DELETE http://localhost:50268/odata/Umbrella/Employees/$ref?$id=http://localhost:50268/odata/Employees(1111) ");
            Employee employeeToDelete = clientContext.Employees.Where(e => e.ID == newEmployee1.ID).Single();
            clientContext.DeleteLink(company, "Employees", employeeToDelete);
            await clientContext.SaveChangesAsync();

            // Call unbound function
            employeesCount = company.GetEmployeesCount();
            Console.WriteLine("After deleting employees of Company, Company employees count is: " + employeesCount);

            // singleton as navigation target
            Console.WriteLine("Associate Company to Employees(1)");
            var employee = clientContext.Employees.Where(e => e.ID == 1).Single();
            clientContext.SetLink(employee, "Company", company);
            await clientContext.SaveChangesAsync();

            await clientContext.LoadPropertyAsync(employee, "Company");
            Console.WriteLine("Employees(1)'s Company is: " + employee.Company.Name);

            Console.WriteLine("\nPress any key to quit...");
            Console.ReadLine();
        }
    }
}
