// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Data.Entity;

namespace ODataBatchSample.Models
{
    public class CustomersContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
    }
}