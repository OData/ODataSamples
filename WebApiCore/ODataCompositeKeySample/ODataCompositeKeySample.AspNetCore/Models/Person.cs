// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace ODataCompositeKeySample.Models
{
    public class Person
    {
        [Key]
        public string FirstName { get; set; }
        [Key]
        public string LastName { get; set; }

        public int Age { get; set; }
    }
}