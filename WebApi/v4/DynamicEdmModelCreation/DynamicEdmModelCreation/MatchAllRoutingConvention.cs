namespace DynamicEdmModelCreation
{
	using System.Linq;
	using System.Net.Http;
	using System.Web.Http.Controllers;
	using System.Web.OData.Routing;
	using System.Web.OData.Routing.Conventions;
	using Microsoft.OData.UriParser;
	using ODataPath = System.Web.OData.Routing.ODataPath;

	public class MatchAllRoutingConvention : IODataRoutingConvention
	{
		public string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
		{
			if (odataPath.PathTemplate == "~/entityset/key/navigation")
			{
				if (controllerContext.Request.Method == HttpMethod.Get)
				{
					NavigationPropertySegment navigationPropertySegment = (NavigationPropertySegment)odataPath.Segments.Last();
					controllerContext.RouteData.Values["navigation"] = navigationPropertySegment.NavigationProperty.Name;

					KeySegment keySegment = (KeySegment)odataPath.Segments[1];
					controllerContext.RouteData.Values[ODataRouteConstants.Key] = keySegment.Keys.First().Value;

					return "GetNavigation";
				}
			}

			return null;
		}

		public string SelectController(ODataPath odataPath, HttpRequestMessage request)
		{
			return (odataPath.Segments.FirstOrDefault() is EntitySetSegment) ? "HandleAll" : null;
		}
	}
}