namespace DynamicEdmModelCreation.Controllers
{
	using System.Linq;
	using System.Web.Http;
	using System.Web.OData;
	using System.Web.OData.Extensions;
	using DynamicEdmModelCreation.DataSource;
	using Microsoft.OData.Edm;
	using Microsoft.OData.UriParser;
	using ODataPath = System.Web.OData.Routing.ODataPath;

	public class HandleAllController : ODataController
	{
		// Get entityset
		public EdmEntityObjectCollection Get()
		{
			// Get entity set's EDM type: A collection type.
			ODataPath path = this.Request.ODataProperties().Path;
			IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
			IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

			// Create an untyped collection with the EDM collection type.
			EdmEntityObjectCollection collection =
				new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

			// Add untyped objects to collection.
			DataSourceProvider.Get((string) this.Request.Properties[Constants.ODataDataSource], entityType, collection);

			return collection;
		}

		// Get entityset(key)
		public IEdmEntityObject Get(string key)
		{
			// Get entity type from path.
			ODataPath path = this.Request.ODataProperties().Path;
			IEdmEntityType entityType = (IEdmEntityType)path.EdmType;

			// Create an untyped entity object with the entity type.
			EdmEntityObject entity = new EdmEntityObject(entityType);

			DataSourceProvider.Get((string) this.Request.Properties[Constants.ODataDataSource], key, entity);

			return entity;
		}

		public IHttpActionResult GetName(string key)
		{
			// Get entity type from path.
			ODataPath path = this.Request.ODataProperties().Path;

			if (path.PathTemplate != "~/entityset/key/property")
			{
				return this.BadRequest("Not the correct property access request!");
			}

			PropertySegment property = path.Segments.Last() as PropertySegment;
			IEdmEntityType entityType = property.Property.DeclaringType as IEdmEntityType;

			// Create an untyped entity object with the entity type.
			EdmEntityObject entity = new EdmEntityObject(entityType);

			DataSourceProvider.Get((string) this.Request.Properties[Constants.ODataDataSource], key, entity);

			object value = DataSourceProvider.GetProperty((string) this.Request.Properties[Constants.ODataDataSource], "Name", entity);

			if (value == null)
			{
				return this.NotFound();
			}

			string strValue = value as string;
			return this.Ok(strValue);
		}

		public IHttpActionResult GetNavigation(string key, string navigation)
		{
			ODataPath path = this.Request.ODataProperties().Path;

			if (path.PathTemplate != "~/entityset/key/navigation")
			{
				return this.BadRequest("Not the correct navigation property access request!");
			}

			NavigationPropertySegment property = path.Segments.Last() as NavigationPropertySegment;
			if (property == null)
			{
				return this.BadRequest("Not the correct navigation property access request!");
			}

			IEdmEntityType entityType = property.NavigationProperty.DeclaringType as IEdmEntityType;

			EdmEntityObject entity = new EdmEntityObject(entityType);

			DataSourceProvider.Get((string)this.Request.Properties[Constants.ODataDataSource], key, entity);

			object value = DataSourceProvider.GetProperty((string)this.Request.Properties[Constants.ODataDataSource], navigation, entity);

			if (value == null)
			{
				return this.NotFound();
			}

			IEdmEntityObject nav = value as IEdmEntityObject;
			if (nav == null)
			{
				return this.NotFound();
			}

			return this.Ok(nav);
		}
	}
}
