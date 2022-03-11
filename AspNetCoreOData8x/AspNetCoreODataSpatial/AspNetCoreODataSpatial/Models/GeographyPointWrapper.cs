using Microsoft.Spatial;
using NetTopologySuite.Geometries;

namespace GeometryWebAPI.Models
{
    public class GeographyPointWrapper
    {
        private readonly Point _dbPoint;
        protected GeographyPointWrapper(Point point)
        {
            _dbPoint = point;
        }

        public static implicit operator GeographyPoint(GeographyPointWrapper wrapper)
        {
            return GeographyPointConverter.ConvertPointTo(wrapper._dbPoint);
        }

        public static implicit operator GeographyPointWrapper(GeographyPoint pt)
        {
            return new GeographyPointWrapper(GeographyPointConverter.ConvertFrom(pt));
        }

        public static implicit operator Point(GeographyPointWrapper wrapper)
        {
            return wrapper._dbPoint;
        }

        public static implicit operator GeographyPointWrapper(Point point)
        {
            return new GeographyPointWrapper(point);
        }
    }
}
