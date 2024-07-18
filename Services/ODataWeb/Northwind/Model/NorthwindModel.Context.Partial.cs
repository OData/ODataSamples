// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWeb.Northwind.Model
{
    #region Namespaces

    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Threading;
    using Azure.Core;
    using Azure.Identity;

    #endregion

    public partial class NorthwindEntities
    {
        public NorthwindEntities()
            : base("name=NorthwindEntities")
        {
            SqlConnection connection = (SqlConnection)Database.Connection;

            if (connection.ConnectionString.IndexOf("Integrated Security=", StringComparison.OrdinalIgnoreCase) != -1
                || connection.ConnectionString.IndexOf("Trusted_Connection=", StringComparison.OrdinalIgnoreCase) != -1
                || connection.ConnectionString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return;
            }

            TokenCredential credential;
            string managedIdentityClientId = ConfigurationManager.AppSettings.Get("ManagedIdentityClientId");
            if (!string.IsNullOrEmpty(managedIdentityClientId))
            {
                // User-assigned managed identity
                DefaultAzureCredentialOptions defaultCredentialOptions = new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = managedIdentityClientId
                };

                credential = new DefaultAzureCredential(defaultCredentialOptions);
            }
            else
            {
                credential = new DefaultAzureCredential();
            }

            AccessToken accessToken = credential.GetToken(
                new TokenRequestContext(new[] { "https://database.windows.net/.default" }), CancellationToken.None);
            connection.AccessToken = accessToken.Token;
        }
    }
}