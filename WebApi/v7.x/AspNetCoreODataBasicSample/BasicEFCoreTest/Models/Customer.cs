using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicEFCoreTest.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public Color FavoriateColor { get; set; }

        public Address HomeAddress { get; set; }

        public Order Order { get; set; }
    }
}
