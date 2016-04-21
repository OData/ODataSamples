namespace ODataSamples.WebApiService.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Routing;
    using Microsoft.Spatial;
    using ODataSamples.WebApiService.DataSource;
    using ODataSamples.WebApiService.Helper;
    using ODataSamples.WebApiService.Models;

    public class AirportsController : ODataController
    {
        // GET odata/Airports
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.Airports.AsQueryable());
        }

        // GET odata/Airports('key')
        [EnableQuery]
        public IHttpActionResult Get([FromODataUri] string key)
        {
            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport != null)
            {
                return Ok(airport);
            }

            return NotFound();
        }

        // GET odata/Airports('key')/Property
        [HttpGet]
        [ODataRoute("Airports({key})/IcaoCode")]
        [ODataRoute("Airports({key})/Name")]
        [ODataRoute("Airports({key})/Location")]
        [ODataRoute("Airports({key})/IataCode")]
        public IHttpActionResult GetAirportProperty([FromODataUri] string key)
        {
            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();

            var propertyValue = ControllerHelper.GetPropertyValueFromModel(airport, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/Airports('key')/Property/$value
        [HttpGet]
        [ODataRoute("Airports({key})/IcaoCode/$value")]
        [ODataRoute("Airports({key})/Name/$value")]
        [ODataRoute("Airports({key})/IataCode/$value")]
        public string GetAirportPropertyValue([FromODataUri] string key)
        {
            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport == null)
            {
                throw new HttpException("Don't find model with key:" + key);
            }

            var segments = this.Url.Request.RequestUri.Segments;
            var propertyName = segments[segments.Length - 2].TrimEnd('/');
            return ControllerHelper.GetPropertyValueFromModel(airport, propertyName).ToString();
        }

        // GET odata/Airports('key')/Location/Property
        [HttpGet]
        [ODataRoute("Airports({key})/Location/Address")]
        [ODataRoute("Airports({key})/Location/City")]
        [ODataRoute("Airports({key})/Location/Loc")]
        public IHttpActionResult GetLocationProperty([FromODataUri] string key)
        {
            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport == null || airport.Location == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(airport.Location, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // POST odata/Airports
        public IHttpActionResult Post(Airport airport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripPinSvcDataSource.Instance.Airports.Add(airport);
            return Created(airport);
        }

        // PUT odata/Airports('key')
        public IHttpActionResult Put([FromODataUri] string key, Airport airport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != airport.IcaoCode)
            {
                return BadRequest("The IcalCode of Airline does not match the key");
            }

            var oldAirport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (oldAirport == null)
            {
                return NotFound();
            }

            TripPinSvcDataSource.Instance.Airports.Remove(oldAirport);
            TripPinSvcDataSource.Instance.Airports.Add(airport);

            return Updated(airport);
        }

        // PATCH odata/Airports('key')
        public IHttpActionResult Patch([FromODataUri] string key, Delta<Airport> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport == null)
            {
                return NotFound();
            }

            patch.Patch(airport);
            return Updated(airport);
        }

        // DELETE odata/Airports('key')
        public IHttpActionResult Delete([FromODataUri] string key)
        {
            var airport = TripPinSvcDataSource.Instance.Airports.SingleOrDefault(item => item.IcaoCode == key);
            if (airport == null)
            {
                return NotFound();
            }

            TripPinSvcDataSource.Instance.Airports.Remove(airport);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #region Function & Action

        // GET odata/GetNearestAirport(lat=value,lon=value)
        [HttpGet]
        [ODataRoute("GetNearestAirport(lat={lat},lon={lon})")]
        public IHttpActionResult GetNearestAirport(double lat, double lon)
        {
            GeographyPoint startPoint = GeographyPoint.Create(lat, lon);

            var url = this.ControllerContext.Request.RequestUri;

            var airports = TripPinSvcDataSource.Instance.Airports;

            double minDistance = 2;
            var nearestAirport = default(Airport);

            foreach (Airport airport in airports)
            {
                double distance = CalculateDistance(startPoint, airport.Location.Loc);
                if (distance < minDistance)
                {
                    nearestAirport = airport;
                    minDistance = distance;
                }
            }
            return Ok(nearestAirport);
        }

        #endregion

        #region Private Methods

        private double CalculateDistance(GeographyPoint p1, GeographyPoint p2)
        {
            // using Haversine formula
            // refer to http://en.wikipedia.org/wiki/Haversine_formula
            double lat1 = Math.PI * p1.Latitude / 180;
            double lat2 = Math.PI * p2.Latitude / 180;
            double lon1 = Math.PI * p1.Longitude / 180;
            double lon2 = Math.PI * p2.Longitude / 180;
            double item1 = (Math.Sin((lat1 - lat2) / 2)) * (Math.Sin((lat1 - lat2) / 2));
            double item2 = Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin((lon1 - lon2) / 2) * Math.Sin((lon1 - lon2) / 2);
            return Math.Asin(Math.Sqrt(item1 + item2));
        }

        #endregion
    }
}
