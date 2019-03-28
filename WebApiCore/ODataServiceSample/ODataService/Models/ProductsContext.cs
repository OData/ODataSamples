// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace ODataService.Models
{
    public class ProductsContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductFamily> ProductFamilies { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        private static bool _created = false;
        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {
            if (!_created)
            {
                this.Database.EnsureDeleted();
                this.Database.EnsureCreated();
                this.Seed();
                _created = true;
            }
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        @"Server=(localdb)\mssqllocaldb;Database=ODataServiceAspNetCore;Integrated Security=True");
        //}

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductFamily>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<Supplier>().Property(p => p.Id).ValueGeneratedNever();

            modelBuilder.Entity<Supplier>().OwnsOne(p => p.Address);

            base.OnModelCreating(modelBuilder);
        }
    }
}
