using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Microsoft.Spatial;

namespace ODataSpatialSample
{
    public class GeographyLineStringWrapper
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("En-Us");

        public static implicit operator GeographyLineStringWrapper(DbGeography dbg)
        {
            if (dbg == null)
            {
                return null;
            }

            if (dbg.SpatialTypeName != "LineString")
            {
                throw new ODataException(String.Format("Need 'LineString type', while input is \'{0}\'",
                    dbg.SpatialTypeName));
            }

            return new GeographyLineStringWrapper(dbg);
        }

        public static implicit operator DbGeography(GeographyLineStringWrapper wrapper)
        {
            if (wrapper == null)
            {
                return null;
            }

            return wrapper._dbGeography;
        }

        public static implicit operator GeographyLineStringWrapper(GeographyLineString lineString)
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

            return new GeographyLineStringWrapper(DbGeography.FromText(sb.ToString()));
        }

        public static implicit operator GeographyLineString(GeographyLineStringWrapper wrapper)
        {
            if (wrapper == null || wrapper._dbGeography == null)
            {
                return null;
            }

            SpatialBuilder builder = SpatialBuilder.Create();
            GeographyPipeline pipleLine = builder.GeographyPipeline;
            pipleLine.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipleLine.BeginGeography(SpatialType.LineString);

            int numPoints = wrapper._dbGeography.PointCount ?? 0;
            if (numPoints > 0)
            {
                DbGeography point = wrapper._dbGeography.PointAt(1);
                pipleLine.BeginFigure(new GeographyPosition(point.Latitude ?? 0, point.Latitude ?? 0, point.Elevation, point.Measure));

                for (int n = 2; n <= numPoints; n++)
                {
                    point = wrapper._dbGeography.PointAt(n);
                    pipleLine.LineTo(new GeographyPosition(point.Latitude ?? 0, point.Latitude ?? 0, point.Elevation, point.Measure));
                }

                pipleLine.EndFigure();
            }

            pipleLine.EndGeography();
            GeographyLineString lineString = (GeographyLineString)builder.ConstructedGeography;
            return lineString;
        }

        private GeographyLineStringWrapper(DbGeography dbg)
        {
            _dbGeography = dbg;
        }

        private readonly DbGeography _dbGeography;
    }
}
