using System.Collections.Generic;
using System.Linq;

namespace ODataBatchSample.Models
{
    public class CustomersContextInitializer
    {
        public static void Seed(CustomersContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            IList<Customer> customers = Enumerable.Range(0, 10).Select(i =>
                new Customer
                {
                    Id = i,
                    Name = "Name " + i
                }).ToList();
            context.Customers.AddRange(customers);

            context.SaveChanges();
        }
    }
}