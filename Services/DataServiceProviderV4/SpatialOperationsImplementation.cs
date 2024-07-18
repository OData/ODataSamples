// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using Microsoft.Spatial;

    public class SpatialOperationsImplementation : SpatialOperations
    {
        private static double EarthRadiusInMiles = 3956.0;

        public override double Distance(Geography operand1, Geography operand2)
        {
            // Haversine Formula - http://mathforum.org/library/drmath/view/51879.html
            var point1 = operand1 as GeographyPoint;
            var point2 = operand2 as GeographyPoint;

            if (point1 == null || point2 == null)
            {
                throw new NotSupportedException("Only GeographyPoints are supported by this geodistance implementation");
            }

            var deltaLongitude = point2.Longitude - point1.Longitude;
            var deltaLatitude = point2.Latitude - point1.Latitude;

            var a = Math.Pow(Math.Sin(deltaLatitude/2), 2) + Math.Cos(point1.Latitude) * Math.Cos(point2.Latitude) * Math.Pow(Math.Sin(deltaLongitude/2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

            return EarthRadiusInMiles * c;
        }
    }
}
