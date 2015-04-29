using AdventureWorksLTSample.Models;
using Microsoft.Restier.WebApi;
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
