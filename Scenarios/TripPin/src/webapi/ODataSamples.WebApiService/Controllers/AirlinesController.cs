namespace ODataSamples.WebApiService.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.OData;
    using ODataSamples.WebApiService.DataSource;
    using ODataSamples.WebApiService.Helper;
    using ODataSamples.WebApiService.Models;

    public class AirlinesController : ODataController
    {
        // GET odata/Airlines
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.Airlines.AsQueryable());
        }

        // GET odata/Airlines('key')
        [EnableQuery]
        public IHttpActionResult Get([FromODataUri] string key)
        {
            var airline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == key);
            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }

        //GET odata/Airports('key')/Name
        public IHttpActionResult GetName([FromODataUri] string key)
        {
            var airline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == key);
            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline.Name);
        }

        // POST odata/Airlines
        public IHttpActionResult Post(Airline airline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripPinSvcDataSource.Instance.Airlines.Add(airline);
            return Created(airline);
        }

        // PUT odata/Airlines('key')
        public IHttpActionResult Put([FromODataUri] string key, Airline airline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != airline.AirlineCode)
            {
                return BadRequest("The IcalCode of Airline does not match the key");
            }

            var oldAirline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == key);
            if (oldAirline == null)
            {
                return NotFound();
            }

            TripPinSvcDataSource.Instance.Airlines.Remove(oldAirline);
            TripPinSvcDataSource.Instance.Airlines.Add(airline);

            return Updated(airline);
        }

        // PATCH odata/Airlines('key')
        public IHttpActionResult Patch([FromODataUri] string key, Delta<Airline> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == key);
            if (airline == null)
            {
                return NotFound();
            }

            patch.Patch(airline);
            return Updated(airline);
        }

        // DELETE odata/Airlines('key')
        public IHttpActionResult Delete([FromODataUri] string key)
        {
            var airline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == key);
            if (airline == null)
            {
                return NotFound();
            }

            TripPinSvcDataSource.Instance.Airlines.Remove(airline);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
