namespace ODataSamples.WebApiService.Controllers
{
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Routing;
    using ODataSamples.WebApiService.DataSource;
    using ODataSamples.WebApiService.Helper;
    using ODataSamples.WebApiService.Models;

    public class SingletonController : ODataController
    {
        // GET odata/Me
        [HttpGet]
        [ODataRoute("Me")]
        public IHttpActionResult GetMe()
        {
            return Ok(TripPinSvcDataSource.Instance.Me);
        }

        // GET odata/Me/Property
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("Me/AddressInfo")]
        [ODataRoute("Me/Emails")]
        [ODataRoute("Me/Trips")]
        [ODataRoute("Me/Friends")]
        public IHttpActionResult GetMeCollectionProperty()
        {
            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(TripPinSvcDataSource.Instance.Me, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/Me/Property
        [HttpGet]
        [ODataRoute("Me/Gender")]
        [ODataRoute("Me/UserName")]
        [ODataRoute("Me/LastName")]
        [ODataRoute("Me/FirstName")]
        public IHttpActionResult GetMeProperty()
        {
            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(TripPinSvcDataSource.Instance.Me, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // TODO : Should test ETag in singleton.
        // PUT odata/Me
        [HttpPut]
        [ODataRoute("Me")]
        public IHttpActionResult PutMe(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripPinSvcDataSource.Instance.Me = person;
            return Updated(person);
        }

        // PATCH odata/Me
        [HttpPatch]
        [ODataRoute("Me")]
        public IHttpActionResult PatchMe(Delta<Person> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            patch.Patch(TripPinSvcDataSource.Instance.Me);
            return Updated(TripPinSvcDataSource.Instance.Me);
        }
    }
}