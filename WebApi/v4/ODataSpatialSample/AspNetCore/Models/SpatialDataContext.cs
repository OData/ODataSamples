// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace ODataSpatialSample.Models
{
    public class SpatialDataContext : DbContext
    {
        public SpatialDataContext(DbContextOptions<SpatialDataContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
