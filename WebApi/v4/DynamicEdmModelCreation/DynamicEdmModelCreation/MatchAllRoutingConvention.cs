using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace DynamicEdmModelCreation
{
    public class MatchAllRoutingConvention : IODataRoutingConvention
    {
        public string SelectAction(
            ODataPath odataPath,
            HttpControllerContext controllerContext,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath.PathTemplate == "~/entityset/key/navigation")
            {
                if (controllerContext.Request.Method == HttpMethod.Get)
                {
                    NavigationPathSegment navigationPathSegment = (NavigationPathSegment)odataPath.Segments.Last();

                    controllerContext.RouteData.Values["navigation"] = navigationPathSegment.NavigationProperty.Name;

                    KeyValuePathSegment keyValueSegment = (KeyValuePathSegment)odataPath.Segments[1];
                    controllerContext.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment.Value;

                    return "GetNavigation";
                }
            }

            return null;
        }

        public string SelectController(ODataPath odataPath, HttpRequestMessage request)
        {
            return (odataPath.Segments.FirstOrDefault() is EntitySetPathSegment) ? "HandleAll" : null;
        }
    }
}