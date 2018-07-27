// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;

namespace ODataActionSample.Model
{
    public class Movie
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public bool IsCheckedOut
        {
            get { return DueDate.HasValue; }
        }
    }
}
