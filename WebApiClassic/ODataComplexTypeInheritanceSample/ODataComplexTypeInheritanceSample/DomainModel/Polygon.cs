// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ODataComplexTypeInheritanceSample
{
    public class Polygon : Shape
    {
        public IList<Point> Vertexes { get; set; }
        public Polygon()
        {
            Vertexes = new List<Point>();
        }
    }
}
