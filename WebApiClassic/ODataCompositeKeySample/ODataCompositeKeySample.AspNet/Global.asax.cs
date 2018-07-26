using System.Web.Http;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using ODataCompositeKeySample.Models;

namespace ODataCompositeKeySample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            var mb = new ODataConventionModelBuilder(config);
            mb.EntitySet<Person>("People");

            // Routing convention for composite keys of entity type is built-in for WebApi 6.x+.
            var conventions = ODataRoutingConventions.CreateDefault();

            config.MapODataServiceRoute(
                routeName: "OData",
                routePrefix: null,
                model: mb.GetEdmModel(),
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: conventions);
        }
    }
}