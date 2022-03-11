using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace GeometryWebAPI.Models
{
    public class DataSource
    {
        private static DataSource instance = new DataSource();
        public static DataSource Instance => instance;
        public List<Customer> Customers { get; set; }

        private DataSource()
        {
            this.Reset();
            this.Initialize();
        }
        public void Reset()
        {
            this.Customers = new List<Customer>();
        }

        public void Initialize()
        {
            Point seattle = new Point(-122.333056, 47.609722) { SRID = 4326 };
            Point redmond = new Point(-122.123889, 47.669444) { SRID = 4326 };

            this.Customers.AddRange(new List<Customer>()
            {
                new Customer()
                {
                    Id = 1,
                    Name = "Customer 1",
                    Loc = seattle
                },
                new Customer()
                {
                    Id = 2,
                    Name = "Customer 2",
                    Loc = redmond
                }
            });
        }
    }
}
