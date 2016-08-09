using System.Web.Http;
using AdventureWorksLTSample.Models;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData.Batch;
using Microsoft.Restier.Publishers.OData;

namespace AdventureWorksLTSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            RegisterAdventureWorks(config, GlobalConfiguration.DefaultServer);
        }

        public static async void RegisterAdventureWorks(HttpConfiguration config, HttpServer server)
        {
            await config.MapRestierRoute<EntityFrameworkApi<AdventureWorksContext>>(
                "AdventureWorksLT",
                "api/AdventureWorksLT",
                new RestierBatchHandler(server)
                );
        }
    }
}
