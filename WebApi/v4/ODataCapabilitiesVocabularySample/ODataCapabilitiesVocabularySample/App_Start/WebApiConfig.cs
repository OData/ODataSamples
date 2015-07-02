using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using CapabilitiesVocabulary;

namespace ODataCapabilitiesVocabularySample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("OData1", "odata/non-cap", GetExplicitEdmModel());
            config.MapODataServiceRoute("OData2", "odata/cap", GetEdmModel());
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
