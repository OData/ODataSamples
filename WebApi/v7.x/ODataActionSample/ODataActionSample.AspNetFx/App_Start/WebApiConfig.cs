// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.Http;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using ODataActionSample.Model;

namespace ODataActionSample.AspNetFx
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("OData", "odata", GetEdmModel());
        }

        // Builds the EDM model for the OData service, including the OData action definitions.
        private static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            var moviesEntitySet = modelBuilder.EntitySet<Movie>("Movies");

            // Now add actions.

            // CheckOut
            // URI: ~/odata/Movies(1)/ODataActionSample.Models.CheckOut
            ActionConfiguration checkOutAction = modelBuilder.EntityType<Movie>().Action("CheckOut");
            checkOutAction.ReturnsFromEntitySet<Movie>("Movies");

            // ReturnMovie
            // URI: ~/odata/Movies(1)/ODataActionSample.Models.Return
            // Binds to a single entity; no parameters.
            ActionConfiguration returnAction = modelBuilder.EntityType<Movie>().Action("Return");
            returnAction.ReturnsFromEntitySet<Movie>("Movies");

            // CheckOutMany action
            // URI: ~/odata/Movies/ODataActionSample.Models.CheckOutMany
            // Binds to a collection of entities.  This action accepts a collection of parameters.
            ActionConfiguration checkOutManyAction = modelBuilder.EntityType<Movie>().Collection.Action("CheckOutMany");
            checkOutManyAction.CollectionParameter<int>("MovieIDs");
            checkOutManyAction.ReturnsCollectionFromEntitySet<Movie>("Movies");

            // CreateMovie action
            // URI: ~/odata/CreateMovie
            // Unbound action. It is invoked from the service root.
            ActionConfiguration createMovieAction = modelBuilder.Action("CreateMovie");
            createMovieAction.Parameter<string>("Title");
            createMovieAction.ReturnsFromEntitySet<Movie>("Movies");

            modelBuilder.Namespace = typeof(Movie).Namespace;
            return modelBuilder.GetEdmModel();
        }
    }
}
