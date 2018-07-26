// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OData.Client;
using System;

namespace ODataCompositeKeySample.Tests
{
    public partial class Container
    {
        public Container()
            : this(new Uri("http://localhost:28141"))
        {
            this.SendingRequest2 += Container_SendingRequest;
        }

        void Container_SendingRequest(object sender, SendingRequest2EventArgs e)
        {
            Console.WriteLine("{0} {1}", e.RequestMessage.Method, e.RequestMessage.Url);
        }
    }
}
