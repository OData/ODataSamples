using Microsoft.EntityFrameworkCore;

namespace ODataModelAliasingSample.AspNetCore.Model
{
    public class CustomersContext : DbContext
    {
        public CustomersContext(DbContextOptions<CustomersContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerDto> Customers { get; set; }
        public DbSet<OrderDto> Orders { get; set; }
    }
}
