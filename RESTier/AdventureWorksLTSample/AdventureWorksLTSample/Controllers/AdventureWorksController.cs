using AdventureWorksLTSample.Models;
using Microsoft.Restier.WebApi;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Routing;

namespace AdventureWorksLTSample.Controllers
{
    public class AdventureWorksController : ODataDomainController<AdventureWorksDomain>
    {
        private AdventureWorksContext DbContext
        {
            get { return Domain.Context; }
        }

        #region Fallback to Web API for OData
        [ODataRoute("Customers({id})/CompanyName")]
        [ODataRoute("Customers({id})/CompanyName/$value")]
        public IHttpActionResult GetCustomerCompanyName(int id)
        {
            var customer = DbContext.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.CompanyName);
        }

        [ODataRoute("Customers/$count")]
        public IHttpActionResult GetCustomersCount()
        {
            return Ok(this.DbContext.Customers.Count());
        }

        [HttpPut]
        [ODataRoute("Products({id})/Color")]
        public IHttpActionResult UpdateProductColor(int id, [FromBody] string color)
        {
            var product = DbContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Color = color;
            DbContext.SaveChanges();

            return Ok(color);
        }

        [ODataRoute("Customers")]
        public IHttpActionResult GetCustomers()
        {
            return Ok(this.DbContext.Customers.Where(c => c.CustomerID % 3 == 0));
        }
        #endregion

        /// <summary>
        /// Attribute routing to enable $count
        /// </summary>
        /// <returns>IHttpActionResult of count</returns>
        [ODataRoute("Products/$count")]
        public async Task<IHttpActionResult> GetProductsCount()
        {
            return Ok(await this.Domain.GetProductsCountAsync());
        }

        /// <summary>
        /// Attribute routing to enable $count
        /// </summary>
        /// <returns>IHttpActionResult of count</returns>
        [ODataRoute("ColoredProducts/$count")]
        public async Task<IHttpActionResult> GetColoredProductsCount()
        {
            return Ok(await this.Domain.GetColoredProductsCountAsync());
        }
    }
}
