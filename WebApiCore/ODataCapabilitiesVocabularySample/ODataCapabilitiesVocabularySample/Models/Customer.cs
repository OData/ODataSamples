// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.OData.Query;

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