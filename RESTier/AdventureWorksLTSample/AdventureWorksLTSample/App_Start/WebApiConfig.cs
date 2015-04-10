using System.Web.Http;
using AdventureWorksLTSample.Controllers;
using Microsoft.Restier.WebApi;
using Microsoft.Restier.WebApi.Batch;

namespace AdventureWorksLTSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            RegeiterAdventureWorks(config, GlobalConfiguration.DefaultServer);
        }

        public static async void RegeiterAdventureWorks(HttpConfiguration config, HttpServer server)
        {
            await config.MapODataDomainRoute<AdventureWorksController>(
                "AdventureWorksLT",
                "AdventureWorksLT",
                new ODataDomainBatchHandler(server)
                );
        }
    }
}
