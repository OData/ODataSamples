using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;

namespace AspNetCoreODataSpatial.Models
{
    public class Customer
    {
        private GeographyPointWrapper _ptWrapper;
        public int Id { get; set; }
        public string Name { get; set; }

        public Point Loc
        {
            get { return _ptWrapper; }
            set { _ptWrapper = value; }
        }

        [NotMapped]
        public GeographyPoint EdmLoc
        {
            get { return _ptWrapper; }
            set { _ptWrapper = value; }
        }
    }

    /*public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Loc { get; set; }
    }*/
}
