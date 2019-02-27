namespace ODataSamples.WebApiService.Controllers
{
    using System.Net;
    using System.Web.Http;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Routing;
    using ODataSamples.WebApiService.DataSource;

    public class CommonController : ODataController
    {
        [ODataRoute("ResetDataSource")]
        public IHttpActionResult ResetDataSource()
        {
            TripPinSvcDataSource.Instance.Reset();
            TripPinSvcDataSource.Instance.Initialize();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
