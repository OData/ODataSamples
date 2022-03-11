using System.Diagnostics;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;

namespace GeometryWebAPI.Models
{
    public class GeographyPointConverter
    {
        public static GeographyPoint ConvertPointTo(Point dbPoint)
        {
            Debug.Assert(dbPoint.GeometryType == "Point");
            double lat = dbPoint.X;
            double lon = dbPoint.Y;
            double? alt = dbPoint.Z;
            double? m = dbPoint.M;
            return GeographyPoint.Create(lat, lon, alt, m);
        }

        public static Point ConvertFrom(GeographyPoint geographyPoint)
        {
            return new Point(geographyPoint.Latitude, geographyPoint.Longitude, (double)geographyPoint.Z);
        }
    }
}
