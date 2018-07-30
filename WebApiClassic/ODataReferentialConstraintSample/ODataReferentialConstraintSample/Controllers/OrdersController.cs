// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData;
using System.Linq;
using System.Web.Http;

namespace ODataReferentialConstraintSample
{
    public class OrdersController : ODataController
    {
        private readonly SampleContext _db = new SampleContext();

        [EnableQuery]
        public IHttpActionResult Get(int key)
        {
            Order order = _db.Orders.FirstOrDefault(c => c.OrderId == key);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
