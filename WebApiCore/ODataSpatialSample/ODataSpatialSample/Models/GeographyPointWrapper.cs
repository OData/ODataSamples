// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using Microsoft.OData;
using Microsoft.Spatial;

namespace ODataSpatialSample.Models
{
    public class GeographyPointWrapper
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("En-Us");

        public static implicit operator GeographyPointWrapper(string dbg)
        {
            if (dbg == null)
            {
                return null;
            }

            if (!dbg.StartsWith("POINT"))
            {
                throw new ODataException(String.Format("Need 'Point type', while input is \'{0}\'", dbg));
            }

            return new GeographyPointWrapper(dbg);
        }

        public static implicit operator String(GeographyPointWrapper wrapper)
        {
            if (wrapper == null)
            {
                return null;
            }

            return wrapper._dbGeography;
        }

        public static implicit operator GeographyPointWrapper(GeographyPoint point)
        {
            if (point == null)
            {
                return null;
            }

            string text = "POINT(" + point.Latitude.ToString(DefaultCulture) + " " +
                     point.Longitude.ToString(DefaultCulture);

            if (point.Z.HasValue)
            {
                text += " " + point.Z.Value;
            }

            if (point.M.HasValue)
            {
                text += " " + point.M.Value;
            }

            text += ")";

            return new GeographyPointWrapper(text);
        }

        public static implicit operator GeographyPoint(GeographyPointWrapper wrapper)
        {
            if (wrapper == null || wrapper._dbGeography == null)
            {
                return null;
            }

            int length = wrapper._dbGeography.Length;
            string subString = wrapper._dbGeography.Substring(6, length - 6 - 1);
            var fields = subString.Split(" ");

            double lat = Double.Parse(fields[0]);
            double lon = Double.Parse(fields[1]);

            double? alt = null;
            if (fields.Length > 2)
            {
                alt = Double.Parse(fields[2]);
            }

            double? m = null;
            if (fields.Length > 3)
            {
                m = Double.Parse(fields[3]);
            }

            return GeographyPoint.Create(lat, lon, alt, m);
        }

        private GeographyPointWrapper(string dbg)
        {
            _dbGeography = dbg;
        }

        private readonly string _dbGeography;
    }
}
