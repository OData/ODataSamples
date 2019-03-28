// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.OData.Client;

namespace ODataService.Models
{
    /// <summary>
    /// Create an override for the generated Container (i.e. container) for the ODataService.Sample
    /// that displays the type of the method and the URL of requests. 
    /// </summary>
    public partial class Container
    {
        public Container()
            : this(new Uri("http://localhost:2947"))
        {
            SendingRequest2 += Container_SendingRequest2;
        }

        private void Container_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            Console.WriteLine("\t{0} {1}", e.RequestMessage.Method, e.RequestMessage.Url.ToString());
        }
    }
}
