using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace ODataReferentialConstraintSample
{
    public class CustomersController : ODataController
    {
        private readonly SampleContext _db = new SampleContext();

        [EnableQuery]
        public IHttpActionResult Get(int key)
        {
            Customer customer = _db.Customers.FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        public IHttpActionResult Delete(int key)
        {
            Customer customer = _db.Customers.FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            _db.Customers.Remove(customer);
            _db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ODataRoute("ResetDataSource")]
        public IHttpActionResult ResetDataSource()
        {
            if (_db.Database.Exists())
            {
                _db.Database.Delete();
                _db.Database.Create();
            }

            Generate();
            return Ok();
        }

        private void Generate()
        {
            int orderId = 1;
            for (int i = 1; i <= 5; i++)
            {
                Customer customer = new Customer
                {
                    Id = i,
                    Name = "Customer #" + i,
                    Orders = Enumerable.Range(1, 3).Select(e =>
                        new Order
                        {
                            OrderId = orderId,
                            OrderName = "Order #" + orderId++,
                            CustomerId = i
                        }).ToList()
                };

                foreach (var order in customer.Orders)
                {
                    order.Customer = customer;
                }

                _db.Customers.Add(customer);
                _db.Orders.AddRange(customer.Orders);
            }

            _db.SaveChanges();
        }
    }
}
