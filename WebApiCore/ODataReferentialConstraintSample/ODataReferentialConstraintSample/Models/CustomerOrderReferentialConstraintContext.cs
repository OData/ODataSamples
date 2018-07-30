// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace ODataReferentialConstraintSample.Models
{
    public class CustomerOrderReferentialConstraintContext : DbContext
    {
        public CustomerOrderReferentialConstraintContext(DbContextOptions<CustomerOrderReferentialConstraintContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }


        public DbSet<Order> Orders { get; set; }

    }
}
