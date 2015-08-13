using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using ODataAlternateKeySamples.Models;

namespace ODataAlternateKeySamples.Controllers
{
    public class OrdersController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(AlternateKeysDataSource.Orders);
        }

        [HttpGet]
        [ODataRoute("Orders({orderKey})")]
        public IHttpActionResult GetOrderByPrimitiveKey(int orderKey)
        {
            foreach (var order in AlternateKeysDataSource.Orders)
            {
                object value;
                if (order.TryGetPropertyValue("OrderId", out value))
                {
                    int intKey = (int)value;
                    if (orderKey == intKey)
                    {
                        return Ok(order);
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [ODataRoute("Orders(Name={orderName})")]
        public IHttpActionResult GetOrderByName([FromODataUri]string orderName)
        {
            foreach (var order in AlternateKeysDataSource.Orders)
            {
                object value;
                if (order.TryGetPropertyValue("Name", out value))
                {
                    string stringKey = (string)value;
                    if (orderName == stringKey)
                    {
                        return Ok(order);
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [ODataRoute("Orders(Token={token})")]
        public IHttpActionResult GetOrderByToken([FromODataUri]Guid token)
        {
            foreach (var order in AlternateKeysDataSource.Orders)
            {
                object value;
                if (order.TryGetPropertyValue("Token", out value))
                {
                    Guid guidKey = (Guid)value;
                    if (token == guidKey)
                    {
                        return Ok(order);
                    }
                }
            }

            return NotFound();
        }
    }
}