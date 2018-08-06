using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using SampleService2.Models;
using Microsoft.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SampleService2.Controllers
{
    public class OrdersController : ODataController
    {
        // GET: odata/Orders
        public IActionResult GetOrders(ODataQueryOptions<Order> queryOptions)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // GET: odata/Orders(5)
        public IActionResult GetOrder([FromODataUri] int key, ODataQueryOptions<Order> queryOptions)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // PUT: odata/Orders(5)
        public IActionResult Put([FromODataUri] int key, Delta<Order> delta)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // POST: odata/Orders
        public IActionResult Post(Order order)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch([FromODataUri] int key, Delta<Order> delta)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // DELETE: odata/Orders(5)
        public IActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
