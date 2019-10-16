// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataService.Models;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller implements support for Suppliers EntitySet.
    /// It does not implement everything, it only supports Query, Get by Key and Create, 
    /// by handling these requests:
    /// 
    /// GET /Suppliers
    /// GET /Suppliers(key)
    /// GET /Suppliers?$filter=..&$orderby=..&$top=..&$skip=..
    /// POST /Suppliers
    /// </summary>
    public class SuppliersController : ODataController
    {
        private ProductsContext _db;

        public SuppliersController(ProductsContext context)
        {
            _db = context;
        }

        public IQueryable<Supplier> Get()
        {
            return _db.Suppliers;
        }

        [EnableQuery]
        public SingleResult<Supplier> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_db.Suppliers.Where(s => s.Id == key));
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Post(Supplier supplier)
        {
            supplier.ProductFamilies = null;

            Supplier addedSupplier = _db.Suppliers.Add(supplier).Entity;
            await _db.SaveChangesAsync();

            return Created(addedSupplier);
        }

        /*
        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
                _db = null;
            }

            base.Dispose(disposing);
        }*/
    }
}
