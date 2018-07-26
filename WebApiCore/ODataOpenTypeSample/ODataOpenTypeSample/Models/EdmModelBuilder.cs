// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace ODataOpenTypeSample
{
    public static class EdmModelBuilder
    {
        private static IEdmModel _edmModel;

        public static IEdmModel GetModel()
        {
            if (_edmModel == null)
            {
                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<Account>("Accounts");
                builder.EnumType<Gender>();

                builder.Namespace = typeof(Account).Namespace;

                _edmModel = builder.GetEdmModel();
            }

            return _edmModel;
        }
    }
}
