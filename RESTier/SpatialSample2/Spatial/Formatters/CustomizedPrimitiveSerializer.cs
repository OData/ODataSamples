// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Data.Entity.Spatial;
using System.Web.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData.Formatter;

namespace Microsoft.OData.Service.Sample.Spatial2.Formatters
{
    public class CustomizedPrimitiveSerializer : RestierPrimitiveSerializer
    {
        private const string GeographyTypeNamePoint = "Point";
        private const string GeographyTypeNameLineString = "LineString";
        private const string GeographyTypeNamePolygon = "Polygon";

        public override ODataPrimitiveValue CreateODataPrimitiveValue(object graph, IEdmPrimitiveTypeReference primitiveType,
            ODataSerializerContext writeContext)
        {
            var dbGeographyValue = graph as DbGeography;
            if (dbGeographyValue != null)
            {
                // The logic is not included in Restier as publisher layer does not touch provider layer type
                if (dbGeographyValue.SpatialTypeName == GeographyTypeNamePoint)
                {
                    graph = dbGeographyValue.ToGeographyPoint();
                }
                else if (dbGeographyValue.SpatialTypeName == GeographyTypeNameLineString)
                {
                    graph = dbGeographyValue.ToGeographyLineString();
                }
                else if (dbGeographyValue.SpatialTypeName == GeographyTypeNamePolygon)
                {
                    // TODO, convert original value and return converted value
                }
            }

            return base.CreateODataPrimitiveValue(graph, primitiveType, writeContext);
        }
    }
}