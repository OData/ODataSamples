// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Microsoft.Spatial;

namespace Microsoft.OData.Service.Sample.Spatial.Models
{
    public class Person
    {
        public long PersonId { get; set; }

        public string UserName { get; set; }

        [NotMapped]
        public string FullName {
            get
            {
                return FirstName+ " "+LastName;
            }
            set
            {
                var strArray = value.Split(' ');
                if (strArray.Length == 2)
                {
                    FirstName = strArray[0];
                    LastName = strArray[1];
                }
            }
        }

        [Required]
        public string FirstName { get; set; }

        [MaxLength(26), MinLength(1)]
        public string LastName { get; set; }

        private GeographyPointWrapper _pointWrapper;
        private GeographyLineStringWrapper _lineStringWrapper;

        [Column("PointLocation")]
        public DbGeography DbLocation
        {
            get { return _pointWrapper; }
            set { _pointWrapper = value; }
        }

        [NotMapped]
        public GeographyPoint PointLocation
        {
            get { return _pointWrapper; }
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
