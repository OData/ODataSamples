﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Trippin
{
    public class Employee : Person
    {
        public virtual ICollection<Person> Peers { get; set; }

        public long Cost { get; set; }

        public Employee()
        {
            Peers = new List<Person>();
        }
    }
}
