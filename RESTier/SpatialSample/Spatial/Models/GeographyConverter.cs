using System;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Text;
using Microsoft.OData.Core;
using Microsoft.Spatial;

namespace Microsoft.OData.Service.Sample.Spatial.Models
{
    public static class GeographyConverter
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("En-Us");

        public static GeographyPoint ToGeographyPoint(this DbGeography dbg)
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

            double lat = dbg.Latitude ?? 0;
            double lon = dbg.Longitude ?? 0;
            double? alt = dbg.Elevation;
            double? m = dbg.Measure;
            return GeographyPoint.Create(lat, lon, alt, m);
        }

        public static DbGeography ToDbGeography(this GeographyPoint point)
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

            return DbGeography.FromText(text);
        }

        public static GeographyLineString ToGeographyLineString(this DbGeography geography)
        {
            if (geography == null)
            {
                return null;
            }

            if (geography.SpatialTypeName != "LineString")
            {
                throw new ODataException(String.Format("Need 'LineString type', while input is \'{0}\'",
                    geography.SpatialTypeName));
            }

            SpatialBuilder builder = SpatialBuilder.Create();
            GeographyPipeline pipleLine = builder.GeographyPipeline;
            pipleLine.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipleLine.BeginGeography(SpatialType.LineString);

            int numPoints = geography.PointCount ?? 0;
            if (numPoints > 0)
            {
                DbGeography point = geography.PointAt(1);
                pipleLine.BeginFigure(new GeographyPosition(point.Latitude ?? 0, point.Latitude ?? 0, point.Elevation, point.Measure));

                for (int n = 2; n <= numPoints; n++)
                {
                    point = geography.PointAt(n);
                    pipleLine.LineTo(new GeographyPosition(point.Latitude ?? 0, point.Latitude ?? 0, point.Elevation, point.Measure));
                }

                pipleLine.EndFigure();
            }

            pipleLine.EndGeography();
            GeographyLineString lineString = (GeographyLineString)builder.ConstructedGeography;
            return lineString;
        }

        public static DbGeography ToDbGeography(this GeographyLineString lineString)
        {
            if (lineString == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder("LINESTRING(");
            int n = 0;
            foreach (var pt in lineString.Points)
            {
                double lat = pt.Latitude;
                double lon = pt.Longitude;
                double? alt = pt.Z;
                double? m = pt.M;

                string pointStr = lat.ToString(DefaultCulture) + " " + lon.ToString(DefaultCulture);

                if (alt != null)
                {
                    pointStr += " " + alt.Value;
                }

                if (m != null)
                {
                    pointStr += " " + m.Value;
                }

                sb.Append(pointStr);
                n++;
                if (n != lineString.Points.Count)
                {
                    sb.Append(",");
                }
            }
            sb.Append(")");

            return DbGeography.FromText(sb.ToString());
        }
    }
}