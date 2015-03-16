using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace ODataReferentialConstraintSample
{
    public class OrdersController : ODataController
    {
        private readonly SampleContext _db = new SampleContext();

        [EnableQuery]
        public IHttpActionResult Get(int key)
        {
            Order order = _db.Orders.FirstOrDefault(c => c.OrderId == key);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
