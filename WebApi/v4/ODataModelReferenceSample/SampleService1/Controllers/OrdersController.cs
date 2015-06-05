using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using SampleService2.Models;

namespace SampleService1.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using SampleService.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Order>("Orders");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrdersController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();
        private static readonly Order _someOrder = new Order {Id = 1, Price = 123};

        // GET: odata/Orders
        public IHttpActionResult GetOrders(ODataQueryOptions<Order> queryOptions)
        {
            return Ok(new List<Order> { _someOrder });
        }

        // GET: odata/Orders(5)
        public IHttpActionResult GetOrder([FromODataUri] int key, ODataQueryOptions<Order> queryOptions)
        {
            if (key == 1)
            {
                return Ok(_someOrder);
            }

            return NotFound();
        }

        // PUT: odata/Orders(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Order> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Put(order);

            // TODO: Save the patched entity.

            // return Updated(order);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // POST: odata/Orders
        public IHttpActionResult Post(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Add create logic here.

            // return Created(order);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Order> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Patch(order);

            // TODO: Save the patched entity.

            // return Updated(order);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // DELETE: odata/Orders(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            // TODO: Add delete logic here.

            // return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
