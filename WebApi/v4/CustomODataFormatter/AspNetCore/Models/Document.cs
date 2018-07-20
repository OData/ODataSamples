// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace CustomODataFormatter.Models
{
    public class Document
    {
        public Document()
        {
        }

        public Document(Document d)
        {
            ID = d.ID;
            Name = d.Name;
            Content = d.Content;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        [NotMapped]
        public double Score { get; set; }
    }
}
