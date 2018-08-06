using System.Net;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData;
using SampleService1.Models;

namespace SampleService1.Controllers
{
    public class ProductsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        // GET: odata/Products
        public IActionResult GetProducts(ODataQueryOptions<Product> queryOptions)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // GET: odata/Products(5)
        public IActionResult GetProduct([FromODataUri] int key, ODataQueryOptions<Product> queryOptions)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // PUT: odata/Products(5)
        public IActionResult Put([FromODataUri] int key, Delta<Product> delta)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // POST: odata/Products
        public IActionResult Post(Product product)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // PATCH: odata/Products(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch([FromODataUri] int key, Delta<Product> delta)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // DELETE: odata/Products(5)
        public IActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
