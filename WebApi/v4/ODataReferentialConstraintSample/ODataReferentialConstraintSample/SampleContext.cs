using System.Data.Entity;

namespace ODataReferentialConstraintSample
{
    public class SampleContext : DbContext
    {
        public static string ConnectionString = @"Data Source=(LocalDb)\v11.0;Integrated Security=True;Initial Catalog=ODataReferentialConstraintSample";

        public SampleContext()
            : base(ConnectionString)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }
    }
}
