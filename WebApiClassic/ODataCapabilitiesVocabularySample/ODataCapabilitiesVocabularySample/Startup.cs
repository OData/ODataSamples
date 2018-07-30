// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.Http;
using CapabilitiesVocabulary;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Owin;

namespace ODataCapabilitiesVocabularySample
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
            config.MapODataServiceRoute("OData1", "odata/non-cap", GetExplicitEdmModel());
            config.MapODataServiceRoute("OData2", "odata/cap", GetEdmModel());
            appBuilder.UseWebApi(config);
        }

        private static IEdmModel GetExplicitEdmModel()
        {
            var builder = new ODataModelBuilder();

            // enum type "Color"
            var color = builder.EnumType<Color>();
            color.Member(Color.Red);
            color.Member(Color.Green);
            color.Member(Color.Blue);
            color.Member(Color.Yellow);
            color.Member(Color.Pink);
            color.Member(Color.Purple);

            // complex type "Address"
            var address = builder.ComplexType<Address>();
            address.Property(a => a.City);
            address.Property(a => a.Street);

            // entity type "Customer"
            var customer = builder.EntityType<Customer>().HasKey(c => c.CustomerId);
            customer.Property(c => c.Name);
            customer.Property(c => c.Token);
            // customer.Property(c => c.Email).IsNotNavigable(); // you can call Fluent API
            customer.Property(c => c.Email);
            customer.CollectionProperty(c => c.Addresses);
            customer.CollectionProperty(c => c.FavoriateColors);
            customer.HasMany(c => c.Orders);

            // entity type "Order"
            var order = builder.EntityType<Order>().HasKey(o => o.OrderId);
            order.Property(o => o.Price);

            // entity sets
            builder.EntitySet<Customer>("Customers").HasManyBinding(c => c.Orders, "Orders");
            builder.EntitySet<Order>("Orders");

            return builder.GetEdmModel();
        }

        // Builds the EDM model
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Order>("Orders");
            return builder.GetEdmModel();
        }
    }
}
