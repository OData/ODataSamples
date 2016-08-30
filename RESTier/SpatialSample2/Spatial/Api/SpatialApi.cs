// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Formatter.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Service.Sample.Spatial2.Formatters;
using Microsoft.OData.Service.Sample.Spatial2.Models;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Providers.EntityFramework;

namespace Microsoft.OData.Service.Sample.Spatial2.Api
{
    public class SpatialApi : EntityFrameworkApi<SpatialModel>
    {
        protected static new IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
        {
            return EntityFrameworkApi<SpatialModel>.ConfigureApi(apiType, services)
                .AddService<IModelBuilder, SpatialModelExtender>()
                .AddSingleton<ODataSerializerProvider, CustomizedSerializerProvider>();
        }


        private class SpatialModelExtender : IModelBuilder
        {
            public Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
            {
                return Task.FromResult(CreateODataServiceModel("Microsoft.OData.Service.Sample.Spatial.Models"));
            }

            private static IEdmModel CreateODataServiceModel(string ns)
            {
                EdmModel model = new EdmModel();
                var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
                model.AddElement(defaultContainer);
                
                var personType = new EdmEntityType(ns, "Person");
                var personIdProperty = new EdmStructuralProperty(personType, "PersonId", EdmCoreModel.Instance.GetInt64(false));
                personType.AddProperty(personIdProperty);
                personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
                personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", EdmCoreModel.Instance.GetString(false)));
                personType.AddProperty(new EdmStructuralProperty(personType, "LastName", EdmCoreModel.Instance.GetString(false)));
                personType.AddProperty(new EdmStructuralProperty(personType, "UserName", EdmCoreModel.Instance.GetString(true)));
                personType.AddProperty(new EdmStructuralProperty(personType, "DbLocation", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true)));
                personType.AddProperty(new EdmStructuralProperty(personType, "DbLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, true)));

                model.AddElement(personType);
                var personSet = new EdmEntitySet(defaultContainer, "People", personType);
                defaultContainer.AddElement(personSet);

                model.SetAnnotationValue(personType, new ClrTypeAnnotation(typeof(Person)));

                return model;
            }
        }

        public SpatialApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}