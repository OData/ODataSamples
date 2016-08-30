// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Web.OData.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Service.Sample.Spatial.Models;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Providers.EntityFramework;

namespace Microsoft.OData.Service.Sample.Spatial.Api
{
    public class SpatialApi : EntityFrameworkApi<SpatialModel>
    {
        protected static new IServiceCollection ConfigureApi(Type apType, IServiceCollection services)
        {
            return EntityFrameworkApi<SpatialModel>.ConfigureApi(apType, services)
                .AddService<IModelBuilder, SpatialModelExtender>();
        }


        private class SpatialModelExtender : IModelBuilder
        {
            public Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
            {
                var builder = new ODataConventionModelBuilder();

                builder.EntitySet<Person>("People");
                var entityConfiguration = builder.StructuralTypes.First(t => t.ClrType == typeof(Person));
                entityConfiguration.AddProperty(typeof(Person).GetProperty("PointLocation"));
                entityConfiguration.AddProperty(typeof(Person).GetProperty("LineString"));
                entityConfiguration.AddProperty(typeof(Person).GetProperty("FullName"));

                var person = builder.EntityType<Person>();
                person.Ignore(t => t.DbLocation);
                person.Ignore(t => t.DbLineString);
                person.Ignore(t => t.FirstName);
                person.Ignore(t => t.LastName);
                return Task.FromResult(builder.GetEdmModel());
            }
        }

        public SpatialApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}