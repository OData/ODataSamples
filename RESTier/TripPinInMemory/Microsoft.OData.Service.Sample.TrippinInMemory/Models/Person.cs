// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trippin
{
    public enum PersonGender
    {
        Male,
        Female,
        Unknown
    }

    public enum Feature
    {
        Feature1,
        Feature2,
        Feature3,
        Feature4
    }

    public class Person
    {
        [Key]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [MaxLength(26), MinLength(1)]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public PersonGender Gender { get; set; }

        public long? Age { get; set; }

        public ICollection<string> Emails { get; set; }

        public ICollection<Location> AddressInfo { get; set; }

        public Location HomeAddress { get; set; }

        public virtual ICollection<Person> Friends { get; set; }

        public Person BestFriend { get; set; }

        public virtual ICollection<Trip> Trips { get; set; }

        public Feature FavoriteFeature { get; set; }

        public virtual ICollection<Feature> Features { get; set; }

        public Person()
        {
            Emails = new List<string>();
            AddressInfo = new List<Location>();
            Friends = new List<Person>();
            Trips = new List<Trip>();
            Features = new List<Feature>();
        }
    }
}