// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Spatial;

namespace ODataSpatialSample.Models
{
    public class Customer
    {
        private GeographyPointWrapper _pointWrapper;

        public int Id { get; set; }

        public string Name { get; set; }

        [Column("Location")]
        public string DbLocation
        {
            get { return _pointWrapper; }
            set { _pointWrapper = value; }
        }

        [NotMapped]
        public GeographyPoint Location
        {
            get { return _pointWrapper; }
            set { _pointWrapper = value; }
        }
    }
}
