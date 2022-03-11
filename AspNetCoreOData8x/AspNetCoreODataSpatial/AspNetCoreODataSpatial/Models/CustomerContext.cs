using Microsoft.EntityFrameworkCore;

namespace GeometryWebAPI.Models
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
          : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
