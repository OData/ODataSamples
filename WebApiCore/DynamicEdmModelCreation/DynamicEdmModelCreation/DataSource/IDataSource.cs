// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation.DataSource
{
    internal interface IDataSource
    {
        void GetModel(EdmModel model, EdmEntityContainer container);

        void Get(IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection);

        void Get(string key, EdmEntityObject entity);

        object GetProperty(string property, EdmEntityObject entity);
    }
}
