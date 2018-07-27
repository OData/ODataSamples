// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataComplexTypeInheritanceSample.Models
{
    public class Circle : Shape
    {
        public Point Center { get; set; }
        public int Radius { get; set; }

        public override string ToString()
        {
            // {centerX, centerY, radius}
            return "{" + Center.X + "," + Center.Y + "," + Radius + "}";
        }
    }
}
