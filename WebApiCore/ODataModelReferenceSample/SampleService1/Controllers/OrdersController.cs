using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleService2.Models;

namespace SampleService1.Controllers
{
    public class OrdersController : ODataController
    {
        private static readonly Order _someOrder = new Order {Id = 1, Price = 123};

        // GET: odata/Orders
        public IActionResult GetOrders(ODataQueryOptions<Order> queryOptions)
        {
            return Ok(new List<Order> { _someOrder });
        }

        // GET: odata/Orders(5)
        public IActionResult GetOrder([FromODataUri] int key, ODataQueryOptions<Order> queryOptions)
        {
            if (key == 1)
            {
                return Ok(_someOrder);
            }

            return NotFound();
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
