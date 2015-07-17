using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using ODataSamples.WebApiService.DataSource;
using ODataSamples.WebApiService.Models;

namespace ODataSamples.WebApiService.Controllers
{
    public class CustomersController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.Customers);
        }

        [EnableQuery]
        public IHttpActionResult GetFavoriateColors(int key)
        {
            Customer customer = TripPinSvcDataSource.Instance.Customers.FirstOrDefault(e => e.CustomerId == key);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.FavoriateColors);
        }
    }

    public class OrdersController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.Orders);
        }
    }
}