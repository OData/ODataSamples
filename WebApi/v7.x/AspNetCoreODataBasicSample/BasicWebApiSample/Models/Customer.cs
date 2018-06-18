// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace BasicWebApiSample.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public Date EffectiveDate { get; set; }

        public Address HomeAddress { get; set; }

        public Color FavoriateColor { get; set; }

        public Order Order { get; set; }

        public IList<Order> Orders { get; set; }
    }
}
