using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using BoundActionSample.Models;

namespace BoundActionSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.Namespace = "MyNamespace";
            modelBuilder.ContainerName = "MyContainer";
            modelBuilder.EntitySet<MyEntity>("MyEntities");

            var action = modelBuilder.Entity<MyEntity>().Action("MyAction");
            action.Returns<string>();

            foreach (var structuralType in modelBuilder.StructuralTypes)
            {
                // Resets the namespace so that the service contains only 1 namespace.
                structuralType.GetType().GetProperty("Namespace").SetValue(structuralType, "MyNamespace");
            }

            var model = modelBuilder.GetEdmModel();
            config.Routes.MapODataServiceRoute("OData", "odata", model);
        }
    }
}
