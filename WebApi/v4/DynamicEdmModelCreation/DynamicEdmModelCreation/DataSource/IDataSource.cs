﻿namespace DynamicEdmModelCreation.DataSource
{
	using System.Web.OData;
	using Microsoft.OData.Edm;

	internal interface IDataSource
	{
		void GetModel(EdmModel model, EdmEntityContainer container);

		void Get(IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection);

		void Get(string key, EdmEntityObject entity);

		object GetProperty(string property, EdmEntityObject entity);
	}
}
