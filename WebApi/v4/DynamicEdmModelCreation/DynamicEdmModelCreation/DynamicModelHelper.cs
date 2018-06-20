namespace DynamicEdmModelCreation
{
	using System.Collections.Generic;
	using System.Web.Http;
	using System.Web.OData.Routing;
	using System.Web.OData.Routing.Conventions;
	using DynamicEdmModelCreation.DataSource;
	using Microsoft.OData.Edm;
	using System.Linq;
	using System.Web.OData.Extensions;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.OData;
	using ServiceLifetime = Microsoft.OData.ServiceLifetime;


	public static class DynamicModelHelper
	{
		public static ODataRoute CustomMapODataServiceRoute(this HttpConfiguration configuration, string routeName, string routePrefix)
		{
			ODataRoute route = configuration.MapODataServiceRoute(routeName, routePrefix, builder =>
			{
				// Get the model from the datasource of the current request: model-per-pequest.
				builder.AddService<IEdmModel>(ServiceLifetime.Scoped, sp =>
				{
					IHttpRequestMessageProvider requestMessageProvider = sp.GetRequiredService<IHttpRequestMessageProvider>();
					string dataSource = requestMessageProvider.Request.Properties[Constants.ODataDataSource] as string;
					IEdmModel model = DataSourceProvider.GetEdmModel(dataSource);
					return model;
				});

				// Create a request provider for every request. This is a workaround for the missing HttpContext of a self-hosted webapi.
				builder.AddService<IHttpRequestMessageProvider>(ServiceLifetime.Scoped, sp => new HttpRequestMessageProvider());

				// The routing conventions are registered as singleton.
				builder.AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp =>
				{
					IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
					routingConventions.Insert(0, new MatchAllRoutingConvention());
					return routingConventions.ToList().AsEnumerable();
				});
			});

			CustomODataRoute odataRoute = new CustomODataRoute(route.RoutePrefix, new CustomODataPathRouteConstraint(routeName));
			configuration.Routes.Remove(routeName);
			configuration.Routes.Add(routeName, odataRoute);

			return odataRoute;
		}
	}
}