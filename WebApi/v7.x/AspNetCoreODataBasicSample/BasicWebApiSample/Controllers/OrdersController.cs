// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using BasicWebApiSample.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApiSample.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly CustomerOrderContext _context;

        public OrdersController(CustomerOrderContext context)
        {
            _context = context;
            CustomersController.Generate(_context);
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Orders);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.Orders.FirstOrDefault(c => c.Id == key));
        }
    }
}
