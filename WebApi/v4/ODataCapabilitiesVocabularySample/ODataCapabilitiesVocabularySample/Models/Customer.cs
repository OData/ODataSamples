using System;
using System.Collections.Generic;
using System.Web.OData.Query;

namespace CapabilitiesVocabulary
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        [NotFilterable]
        [NotSortable]
        public Guid Token { get; set; }

        [NotNavigable]
        public string Email { get; set; }

        [NotCountable]
        public IList<Address> Addresses { get; set; }

        [NotCountable]
        public IList<Color> FavoriateColors { get; set; }

        [NotExpandable]
        public IEnumerable<Order> Orders { get; set; }
    }
}