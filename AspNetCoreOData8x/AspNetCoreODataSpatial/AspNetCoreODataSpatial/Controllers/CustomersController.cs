using System.Linq;
using GeometryWebAPI.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NetTopologySuite.Geometries;

namespace GeometryWebAPI.Controllers
{
    public class CustomersController : ODataController
    {
        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return DataSource.Instance.Customers.AsQueryable<Customer>();
        }

        [EnableQuery]
        public Customer Get(int key)
        {
            Point redmond = new Point(-122.123889, 47.669444) { SRID = 4326 };
            return DataSource.Instance.Customers.Where(b => b.Id == key).Single();
        }
    }
}
