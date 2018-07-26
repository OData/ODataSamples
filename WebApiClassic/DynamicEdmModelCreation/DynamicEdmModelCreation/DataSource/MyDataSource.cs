// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation.DataSource
{
    internal class MyDataSource : IDataSource
    {
        public void GetModel(EdmModel model, EdmEntityContainer container)
        {
            EdmEntityType product = new EdmEntityType("ns", "Product");
            product.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            EdmStructuralProperty key = product.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            product.AddKeys(key);
            model.AddElement(product);
            EdmEntitySet products = container.AddEntitySet("Products", product);

            EdmEntityType detailInfo = new EdmEntityType("ns", "DetailInfo");
            detailInfo.AddKeys(detailInfo.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            detailInfo.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
            model.AddElement(detailInfo);
            EdmEntitySet detailInfos = container.AddEntitySet("DetailInfos", product);

            EdmNavigationProperty detailInfoNavProp = product.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "DetailInfo",
                    TargetMultiplicity = EdmMultiplicity.One,
                    Target = detailInfo
                });
            products.AddNavigationTarget(detailInfoNavProp, detailInfos);
        }

        public void Get(IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection)
        {
            EdmEntityObject entity = new EdmEntityObject(entityType);
            entity.TrySetPropertyValue("Name", "abc");
            entity.TrySetPropertyValue("ID", 1);
            entity.TrySetPropertyValue("DetailInfo", CreateDetailInfo(88, "abc_detailinfo", entity.ActualEdmType));

            collection.Add(entity);
            entity = new EdmEntityObject(entityType);
            entity.TrySetPropertyValue("Name", "def");
            entity.TrySetPropertyValue("ID", 2);
            entity.TrySetPropertyValue("DetailInfo", CreateDetailInfo(99, "def_detailinfo", entity.ActualEdmType));

            collection.Add(entity);
        }

        public void Get(string key, EdmEntityObject entity)
        {
            entity.TrySetPropertyValue("Name", "abc");
            entity.TrySetPropertyValue("ID", int.Parse(key));
            entity.TrySetPropertyValue("DetailInfo", CreateDetailInfo(88, "abc_detailinfo", entity.ActualEdmType));
        }

        public object GetProperty(string property, EdmEntityObject entity)
        {
            object value;
            entity.TryGetPropertyValue(property, out value);
            return value;
        }

        private IEdmEntityObject CreateDetailInfo(int id, string title, IEdmStructuredType edmType)
        {
            IEdmNavigationProperty navigationProperty = edmType.DeclaredProperties.OfType<EdmNavigationProperty>().FirstOrDefault(e => e.Name == "DetailInfo");
            if (navigationProperty == null)
            {
                return null;
            }

            EdmEntityObject entity = new EdmEntityObject(navigationProperty.ToEntityType());
            entity.TrySetPropertyValue("ID", id);
            entity.TrySetPropertyValue("Title", title);
            return entity;
        }
    }
}
