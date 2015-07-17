using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData.Query;
using ODataSamples.WebApiService.Models.Enum;

namespace ODataSamples.WebApiService.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        [NotFilterable]
        public Guid Token { get; set; }

        [NotSortable]
        public string Email { get; set; }

        [NotCountable]
        public IList<Address> Addresses { get; set; }

        [NotCountable]
        public IList<Color> FavoriateColors { get; set; }

        [NotNavigable]
        [NotExpandable]
        public IEnumerable<Order> Orders { get; set; }
    }
}