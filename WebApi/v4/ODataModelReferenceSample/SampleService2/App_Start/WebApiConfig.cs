using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using SampleService2.Models;

namespace SampleService2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Order>("Orders");
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
