﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Microsoft.Spatial;

namespace ODataSpatialSample.Models
{
    public class Customer
    {
        private GeographyPointWrapper _pointWrapper;
        private GeographyLineStringWrapper _lineStringWrapper;

        public int Id { get; set; }

        public string Name { get; set; }

        [Column("Location")]
        public DbGeography DbLocation
        {
            get { return _pointWrapper;}
            set { _pointWrapper = value; }
        }

        [NotMapped]
        public GeographyPoint Location
        {
            get { return _pointWrapper;}
            set { _pointWrapper = value; }
        }

        [Column("LineString")]
        public DbGeography DbLineString
        {
            get { return _lineStringWrapper; }
            set { _lineStringWrapper = value; }
        }

        [NotMapped]
        public GeographyLineString LineString
        {
            get { return _lineStringWrapper; }
            set { _lineStringWrapper = value; }
        }
    }
}
