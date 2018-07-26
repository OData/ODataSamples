// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Web.Http;
using DynamicEdmModelCreation.DataSource;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;

namespace DynamicEdmModelCreation.Controllers
{
    public class HandleAllController : ODataController
    {
        // Get entityset
        public EdmEntityObjectCollection Get()
        {
            // Get entity set's EDM type: A collection type.
            ODataPath path = Request.ODataProperties().Path;
            IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
            IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

            // Create an untyped collection with the EDM collection type.
            EdmEntityObjectCollection collection =
                new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

            // Add untyped objects to collection.
            DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], entityType, collection);

            return collection;
        }

        // Get entityset(key)
        public IEdmEntityObject Get(string key)
        {
            // Get entity type from path.
            ODataPath path = Request.ODataProperties().Path;
            IEdmEntityType entityType = (IEdmEntityType)path.EdmType;

            // Create an untyped entity object with the entity type.
            EdmEntityObject entity = new EdmEntityObject(entityType);

            DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], key, entity);

            return entity;
        }

        public IHttpActionResult GetName(string key)
        {
            // Get entity type from path.
            ODataPath path = Request.ODataProperties().Path;

            if (path.PathTemplate != "~/entityset/key/property")
            {
                return BadRequest("Not the correct property access request!");
            }

            PropertySegment property = path.Segments.Last() as PropertySegment;
            IEdmEntityType entityType = property.Property.DeclaringType as IEdmEntityType;

            // Create an untyped entity object with the entity type.
            EdmEntityObject entity = new EdmEntityObject(entityType);

            DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], key, entity);

            object value = DataSourceProvider.GetProperty((string)Request.Properties[Constants.ODataDataSource], "Name", entity);

            if (value == null)
            {
                return NotFound();
            }

            string strValue = value as string;
            return Ok(strValue);
        }

        public IHttpActionResult GetNavigation(string key, string navigation)
        {
            ODataPath path = Request.ODataProperties().Path;

            if (path.PathTemplate != "~/entityset/key/navigation")
            {
                return BadRequest("Not the correct navigation property access request!");
            }

            NavigationPropertySegment property = path.Segments.Last() as NavigationPropertySegment;
            if (property == null)
            {
                return BadRequest("Not the correct navigation property access request!");
            }

            IEdmEntityType entityType = property.NavigationProperty.DeclaringType as IEdmEntityType;

            EdmEntityObject entity = new EdmEntityObject(entityType);

            DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], key, entity);

            object value = DataSourceProvider.GetProperty((string)Request.Properties[Constants.ODataDataSource], navigation, entity);

            if (value == null)
            {
                return NotFound();
            }

            IEdmEntityObject nav = value as IEdmEntityObject;
            if (nav == null)
            {
                return NotFound();
            }

            return Ok(nav);
        }
    }
}
