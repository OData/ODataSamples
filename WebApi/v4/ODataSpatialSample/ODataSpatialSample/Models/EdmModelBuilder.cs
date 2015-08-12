using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData.Builder;
using Microsoft.OData.Edm;

namespace ODataSpatialSample.Models
{
    public class EdmModelBuilder
    {
        private static IEdmModel _model;

        public static IEdmModel GetEdmModel()
        {
            if (_model != null)
            {
                return _model;
            }

            return _model = BuildEdmModel();
        }

        private static IEdmModel BuildEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Customer>("Customers");

            var cu = builder.StructuralTypes.First(t => t.ClrType == typeof(Customer));
            cu.AddProperty(typeof(Customer).GetProperty("Location"));
            cu.AddProperty(typeof(Customer).GetProperty("LineString"));

            var customer = builder.EntityType<Customer>();
            customer.Ignore(t => t.DbLocation);
            customer.Ignore(t => t.DbLineString);

            return builder.GetEdmModel();
        }
    }
}
