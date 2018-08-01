// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ODataBatchSample.Models
{
    public class CustomersContextInitializer : DropCreateDatabaseAlways<CustomersContext>
    {
        protected override void Seed(CustomersContext context)
        {
            IList<Customer> customers = Enumerable.Range(0, 10).Select(i =>
                new Customer
                {
                    Id = i,
                    Name = "Name " + i
                }).ToList();
            context.Customers.AddRange(customers);
        }
    }
}