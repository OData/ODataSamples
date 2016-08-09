// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

namespace Microsoft.OData.Service.Sample.Spatial2.Models
{
    public class Person
    {
        public long PersonId { get; set; }

        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [MaxLength(26), MinLength(1)]
        public string LastName { get; set; }

        public DbGeography DbLocation { get; set; }

        public DbGeography DbLineString { get; set; }
    }
}
