// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Microsoft.Restier.Providers.EntityFramework;
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

        private DbGeography _dbLocation;
        private DbGeography _lineString;

        [Column("PointLocation")]
        public DbGeography DbLocation
        {
            get { return _dbLocation; }
            set { _dbLocation = value; }
        }

        [NotMapped]
        public GeographyPoint PointLocation
        {
            get { return _dbLocation.ToGeographyPoint(); }
            set { _dbLocation = value.ToDbGeography(); }
        }

        [Column("LineString")]
        public DbGeography DbLineString
        {
            get { return _lineString; }
            set { _lineString = value; }
        }

        [NotMapped]
        public GeographyLineString LineString
        {
            get { return _lineString.ToGeographyLineString(); }
            set { _lineString = value.ToDbGeography(); }
        }
    }
}
