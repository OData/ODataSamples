using System.Web.Http;
using Microsoft.Restier.Publishers.OData.Batch;
using Microsoft.Restier.Publishers.OData.Routing;
using Microsoft.OData.Service.Sample.TrippinInMemory.Models;


namespace Microsoft.OData.Service.Sample.TrippinInMemory
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapRestierRoute<TrippinApi>(
                "TrippinApi",
                null,
                new RestierBatchHandler(GlobalConfiguration.DefaultServer)).Wait();
        }
    }
}
