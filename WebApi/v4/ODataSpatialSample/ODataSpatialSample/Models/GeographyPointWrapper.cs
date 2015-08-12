using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Microsoft.Spatial;

namespace ODataSpatialSample.Models
{
    public class GeographyPointWrapper
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("En-Us");

        public static implicit operator GeographyPointWrapper(DbGeography dbg)
        {
            if (dbg == null)
            {
                return null;
            }

            if (dbg.SpatialTypeName != "Point")
            {
                throw new ODataException(String.Format("Need 'Point type', while input is \'{0}\'",
                    dbg.SpatialTypeName));
            }

            return new GeographyPointWrapper(dbg);
        }

        public static implicit operator DbGeography(GeographyPointWrapper wrapper)
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

            return new GeographyPointWrapper(DbGeography.FromText(text));
        }

        public static implicit operator GeographyPoint(GeographyPointWrapper wrapper)
        {
            if (wrapper == null || wrapper._dbGeography == null)
            {
                return null;
            }

            double lat = wrapper._dbGeography.Latitude ?? 0;
            double lon = wrapper._dbGeography.Longitude ?? 0;
            double? alt = wrapper._dbGeography.Elevation;
            double? m = wrapper._dbGeography.Measure;
            return GeographyPoint.Create(lat, lon, alt, m);
        }

        private GeographyPointWrapper(DbGeography dbg)
        {
            _dbGeography = dbg;
        }

        private readonly DbGeography _dbGeography;
    }
}
