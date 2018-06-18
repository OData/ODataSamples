// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Web.Http;
using BasicWebApiFxSample.Models;
using Microsoft.AspNet.OData;

namespace BasicWebApiFxSample.Controllers
{
    public class OperationsController : ODataController
    {
        private readonly OperationContext _db = new OperationContext();

        public OperationsController()
        {
            if(!_db.Operations.Any())
            {
                Operation o = new Operation
                {
                    OperationDate = DateTime.Now,
                    CreatedAt = DateTimeOffset.Now,
                    Duration = TimeSpan.Zero
                };
                _db.Operations.Add(o);
                _db.SaveChanges();
            }
        }

        [EnableQuery]
        public IQueryable<Operation> Get() => this._db.Operations;

        [EnableQuery]
        public SingleResult<Operation> Get([FromODataUri] int key) =>
            SingleResult.Create(this._db.Operations.Where(p => p.Id == key));

        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}