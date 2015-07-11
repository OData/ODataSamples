using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using MessageApp.Models;

namespace MessageApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var builder = new ODataConventionModelBuilder();
            builder.ComplexType<Extension>();
            builder.EntitySet<Message>("Messages");
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
