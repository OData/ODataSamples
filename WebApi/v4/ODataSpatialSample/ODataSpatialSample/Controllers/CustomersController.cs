using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.OData;
using System.Web.OData.Query;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using ODataSpatialSample.Models;

namespace ODataSpatialSample.Controllers
{
    public class CustomersController : ODataController
    {
        private SpatialDataContext db = new SpatialDataContext();

        /*
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(db.Customers);
        }*/

        public IHttpActionResult Get(ODataQueryOptions<Customer> options)
        {
            IList<Customer> customers = db.Customers.ToList();

            FilterQueryOption filter = options.Filter;

            FilterClause filterClause = filter.FilterClause;
            SingleValueFunctionCallNode functionCall = filterClause.Expression as SingleValueFunctionCallNode;
            if (functionCall != null)
            {
                if (functionCall.Name == "geo.intersects")
                {
                    customers = BindGeoIntersections(functionCall, customers).ToList();
                }
            }

            return Ok(customers);
        }

        public IHttpActionResult Get(int key)
        {
            Customer customer = db.Customers.FirstOrDefault(e => e.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        public IHttpActionResult Post(Customer customer)
        {
            if (customer == null)
            {
                // 5.10 or higher will support to post the spatial type
                return BadRequest("Post the spatial type doesn't support yet!");
            }

            return Ok(customer);
        }

        private IEnumerable<Customer> BindGeoIntersections(SingleValueFunctionCallNode node, IEnumerable<Customer> customers)
        {
            Debug.Assert(node.Name == "geo.intersects");
            Debug.Assert(2 == node.Parameters.Count());

            SingleValuePropertyAccessNode first = node.Parameters.First() as SingleValuePropertyAccessNode;
            ConstantNode second = node.Parameters.Last() as ConstantNode;

            Debug.Assert(first != null && second != null);

            IEdmProperty property = first.Property;
            GeographyPolygon polygon = second.Value as GeographyPolygon;

            Debug.Assert(property != null && polygon != null);

            return DoGeoIntersections(customers, property, polygon);
        }

        private IEnumerable<Customer> DoGeoIntersections(IEnumerable<Customer> customers, IEdmProperty property, GeographyPolygon polygon)
        {
            PropertyInfo propertyInfo = typeof(Customer).GetProperty(property.Name);

            foreach (var customer in customers)
            {
                GeographyPoint point = propertyInfo.GetValue(customer) as GeographyPoint;

                if (point == null)
                {
                    continue;
                }

                bool isIntersection = IsIntersection(point, polygon);
                if (isIntersection)
                {
                    yield return customer;
                }
            }
        }

        // just for simplicity, assume:
        // it's x-y plane
        // polygon hasn't inner rings, it is a rectangle
        private bool IsIntersection(GeographyPoint point, GeographyPolygon polygon)
        {
            var outRing = polygon.Rings.First();
            //int pointNum = outRing.Points.Count;

            double maxLat = double.MinValue;
            double maxLon = double.MinValue;
            double minLat = double.MaxValue;
            double minLon = double.MaxValue;
            foreach (var pt in outRing.Points)
            {
                if (maxLat < pt.Latitude)
                {
                    maxLat = pt.Latitude;
                }

                if (maxLon < pt.Longitude)
                {
                    maxLon = pt.Longitude;
                }

                if (minLat > pt.Latitude)
                {
                    minLat = pt.Latitude;
                }

                if (minLon > pt.Longitude)
                {
                    minLon = pt.Longitude;
                }
            }

            if (point.Latitude < minLat || point.Latitude > maxLat ||
                point.Longitude < minLon || point.Longitude > maxLon)
                return false;

            return true;
        }
    }
}
