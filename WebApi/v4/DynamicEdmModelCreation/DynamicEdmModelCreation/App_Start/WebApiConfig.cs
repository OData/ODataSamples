namespace DynamicEdmModelCreation
{
	using System.Web.Http;
	using System.Web.OData.Extensions;

	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.EnableDependencyInjection();

			config.CustomMapODataServiceRoute("odata", "odata");

			config.AddODataQueryFilter();
		}
	}
}
