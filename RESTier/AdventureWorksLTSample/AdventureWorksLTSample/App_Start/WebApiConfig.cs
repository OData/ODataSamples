using System.Web.Http;
using System.Web.OData.Extensions;
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
            // enable query options for all properties
            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
            await config.MapRestierRoute<EntityFrameworkApi<AdventureWorksContext>>(
                "AdventureWorksLT",
                "api/AdventureWorksLT",
                new RestierBatchHandler(server)
                );
        }
    }
}
