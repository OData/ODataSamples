﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation.DataSource
{
    internal class DataSourceProvider
    {
        public static IEdmModel GetEdmModel(string dataSourceName)
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("ns", "container");
            model.AddElement(container);

            GetDataSource(dataSourceName).GetModel(model, container);

            return model;
        }

        public static void Get(
            string dataSourceName,
            IEdmEntityTypeReference entityType,
            EdmEntityObjectCollection collection)
        {
            GetDataSource(dataSourceName).Get(entityType, collection);
        }

        public static void Get(string dataSourceName, string key, EdmEntityObject entity)
        {
            GetDataSource(dataSourceName).Get(key, entity);
        }

        public static object GetProperty(string dataSourceName, string property, EdmEntityObject entity)
        {
            return GetDataSource(dataSourceName).GetProperty(property, entity);
        }

        private static IDataSource GetDataSource(string dataSourceName)
        {
            dataSourceName = dataSourceName == null ? string.Empty : dataSourceName.ToLowerInvariant();

            switch (dataSourceName)
            {
                case Constants.MyDataSource:
                    return new MyDataSource();
                case Constants.AnotherDataSource:
                    return new AnotherDataSource();
                default:
                    throw new InvalidOperationException(
                        string.Format("Data source: {0} is not registered.", dataSourceName));
            }
        }
    }
}
