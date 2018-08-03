using System.Collections.Generic;
using System.Linq;

namespace ODataModelAliasingSample.AspNetCore.Model
{
    public class CustomersContextInitializer
    {
        public static void Seed(CustomersContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            IList<CustomerDto> customers = Enumerable.Range(1, 10).Select(i => new CustomerDto
            {
                Id = i,
                GivenName = "First name " + i,
                LastName = "Last name " + i,
                Purchases = Enumerable.Range(1, i).Select(j => new OrderDto
                {
                    Id = i * 10 + j,
                    Total = (i * 10 + j) * 3
                }).ToList()
            }).ToList();

            context.Customers.AddRange(customers);

            context.SaveChanges();
        }
    }
}
