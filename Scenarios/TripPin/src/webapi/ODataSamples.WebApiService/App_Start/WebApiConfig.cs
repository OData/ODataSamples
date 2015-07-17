namespace ODataSamples.WebApiService
{
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Batch;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataSamples.WebApiService.Models;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new ETagMessageHandler());
            config.MapODataServiceRoute("odata", null, GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            config.EnsureInitialized();
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Namespace = "ODataSamples.WebApiService.Models";
            builder.ContainerName = "DefaultContainer";

            builder.EntitySet<Person>("People");
            builder.EntitySet<Airport>("Airports");
            builder.EntitySet<Airline>("Airlines");
            builder.Singleton<Person>("Me");

            // Customer - Order scenarios
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Order>("Orders");

            #region Unbound Function & Action

            builder.Action("ResetDataSource");

            //TODO : Double type has some issue.
            var GetNearestAirportFun = builder.Function("GetNearestAirport");
            GetNearestAirportFun.Parameter<double>("lat");
            GetNearestAirportFun.Parameter<double>("lon");
            GetNearestAirportFun.ReturnsFromEntitySet<Airport>("Airports");

            #endregion


            #region Person Function & Action

            var personType = builder.EntityType<Person>();
            personType.Function("GetFavoriteAirline")
                .ReturnsFromEntitySet<Airline>("Airlines");

            //TODO: Fix this issue, the return type can't be "Trip".
            personType.Function("GetFriendsTrips")
                .ReturnsCollectionFromEntitySet<Airline>("Airlines")
                .Parameter<string>("userName");

            var shareTripAction = personType.Action("ShareTrip");
            shareTripAction.Parameter<string>("userName");
            shareTripAction.Parameter<int>("tripId");

            #endregion


            #region Trip Function & Action
            // TODO: The request will fail, should fix this issue.
            // GET odata/People('russellwhyte')/Trips(1001)/ODataSamples.WebApiService.Models.GetInvolvedPeople()
            var tripType = builder.EntityType<Trip>();
            tripType.Function("GetInvolvedPeople")
                .ReturnsCollectionFromEntitySet<Person>("People");

            #endregion


            #region Add Navigation Target

            var edmModel = builder.GetEdmModel();
            var peopleEntitySet = edmModel.EntityContainer.FindEntitySet("People") as EdmEntitySet;
            var meSingleton = edmModel.EntityContainer.FindSingleton("Me") as EdmSingleton;
            var flightEntityType = edmModel.FindDeclaredType("ODataSamples.WebApiService.Models.Flight") as EdmEntityType;

            var propertyAriline = flightEntityType.FindProperty("Airline") as EdmNavigationProperty;
            var propertyFrom = flightEntityType.FindProperty("From") as EdmNavigationProperty;
            var propertyTo = flightEntityType.FindProperty("To") as EdmNavigationProperty;

            var targetAirlines = edmModel.EntityContainer.FindEntitySet("Airlines");
            var targetAirports = edmModel.EntityContainer.FindEntitySet("Airports");

            peopleEntitySet.AddNavigationTarget(propertyAriline, targetAirlines);
            peopleEntitySet.AddNavigationTarget(propertyFrom, targetAirports);
            peopleEntitySet.AddNavigationTarget(propertyTo, targetAirports);

            meSingleton.AddNavigationTarget(propertyAriline, targetAirlines);
            meSingleton.AddNavigationTarget(propertyFrom, targetAirports);
            meSingleton.AddNavigationTarget(propertyTo, targetAirports);

            #endregion

            return edmModel;
        }
    }
}
