namespace ODataSamples.WebApiService.Controllers
{
    using ODataSamples.WebApiService.DataSource;
    using ODataSamples.WebApiService.Helper;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Routing;

    public class MeController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.Me);
        }

        [HttpGet]
        [ODataRoute("Me/AddressInfo")]
        [ODataRoute("Me/Gender")]
        [ODataRoute("Me/Emails")]
        [ODataRoute("Me/UserName")]
        [ODataRoute("Me/Trips")]
        [ODataRoute("Me/LastName")]
        [ODataRoute("Me/FirstName")]
        [ODataRoute("Me/Friends")]
        public IHttpActionResult SelectPropertyValue()
        {
            var Me = TripPinSvcDataSource.Instance.Me;

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            return ControllerHelper.GetPropertyValueFromModel(this, Me, propertyName);
        }

        #region Private methods

        #endregion
    }
}
