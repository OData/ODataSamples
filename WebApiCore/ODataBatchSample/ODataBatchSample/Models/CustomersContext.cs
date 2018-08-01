// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace ODataBatchSample.Models
{
    public class CustomersContext : DbContext
    {

        public CustomersContext(DbContextOptions<CustomersContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}