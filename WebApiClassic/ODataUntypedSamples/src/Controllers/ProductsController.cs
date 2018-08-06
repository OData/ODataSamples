﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace ODataUntypedSample.Controllers
{
    public class ProductsController : ODataController
    {
        private static IQueryable<IEdmEntityObject> Products = Enumerable.Range(0, 10).Select(i =>
            {
                IEdmEntityType productType = (IEdmEntityType)ODataUntypedSample.Model.FindType("NS.Product");
                IEdmEntityTypeReference categoryType = (IEdmEntityTypeReference)productType.FindProperty("Category").Type;

                EdmEntityObject product = new EdmEntityObject(productType);
                product.TrySetPropertyValue("Id", i);
                product.TrySetPropertyValue("Name", "Product " + i);
                product.TrySetPropertyValue("Price", i + 0.01);

                EdmEntityObject category = new EdmEntityObject(categoryType);
                category.TrySetPropertyValue("Id", i % 5);
                category.TrySetPropertyValue("Name", "Category " + (i % 5));
                product.TrySetPropertyValue("Category", category);

                return product;
            }).AsQueryable();

        public EdmEntityObjectCollection Get()
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Collection);

            IEdmCollectionType collectionType = edmType as IEdmCollectionType;
            IEdmEntityType entityType = collectionType.ElementType.Definition as IEdmEntityType;

            IEdmModel model = Request.GetModel();

            ODataQueryContext queryContext = new ODataQueryContext(model, entityType, path);
            ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);

            // Apply the query option on the IQueryable here.

            return new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType), Products.ToList());
        }

        public IEdmEntityObject GetProduct(int key)
        {
            IEdmEntityObject product = Products.Single(p => HasId(p, key));

            return product;
        }

        public IEdmEntityObject GetCategoryFromProduct(int key)
        {
            IEdmEntityObject product = Products.Single(p => HasId(p, key));

            object category;
            if (product.TryGetPropertyValue("Category", out category))
            {
                return (IEdmEntityObject)category;
            }
            else
            {
                return null;
            }
        }

        public IEdmEntityObject Post(IEdmEntityObject entity)
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Collection);

            IEdmEntityTypeReference entityType = (edmType as IEdmCollectionType).ElementType.AsEntity();

            // Do something with the entity object here.

            return entity;
        }

        private bool HasId(IEdmEntityObject product, int key)
        {
            object id;
            return product.TryGetPropertyValue("Id", out id) && (int)id == key;
        }
    }
}
