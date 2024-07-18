// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV4.Northwind
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.SqlClient;
    using System.IO;
    using System.ServiceModel;
    using System.Text;
    using System.Web;
    using Microsoft.OData.Service.Providers;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class NorthwindService : EntityFrameworkDataService<Model.NorthwindEntities>
    {
        #region Database Setup

        /// <summary>
        /// Generate the Northwind Database by calling CreateNW.sql
        /// </summary>
        private static void SetupNorthwindDatabase()
        {
            String dblFilePath = Utils.ResolvePhysicalPath(ConfigurationManager.AppSettings["NW_dbHolderPath"]);

            if (!File.Exists(dblFilePath))
            {
                string dbScriptFilePath = Utils.ResolvePhysicalPath(ConfigurationManager.AppSettings["NW_dbScriptPath"]);
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NorthwindSetup"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        //
                        // Executing the SQL script (Since SqlCommand does not support "GO")
                        //
                        FileStream fs = new FileStream(dbScriptFilePath, FileMode.Open, FileAccess.Read);
                        using (StreamReader r = new StreamReader(fs))
                        {
                            String line = null;
                            StringBuilder batchText = new StringBuilder();
                            while ((line = r.ReadLine()) != null)
                            {
                                if (line.Trim().ToUpper() == "GO")
                                {
                                    DbCommand cmd = conn.CreateCommand();
                                    cmd.CommandText = batchText.ToString();
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandTimeout = 300;
                                    cmd.ExecuteNonQuery();

                                    batchText = new StringBuilder();
                                }
                                else
                                {
                                    batchText.AppendLine(line);
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new DataServiceException(500, "Failed to generate database, the message is: " + ex.Message);
                    }
                }

                // Create a "lock" file so we will not generate the DB again
                File.Create(dblFilePath).Close();
            }
        }

        #endregion

        /// <summary>
        /// Initialize service for the first time
        /// </summary>
        /// <param name="config">Data service configuration</param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            SetupNorthwindDatabase();

            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.None);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.DataServiceBehavior.UrlConventions = DataServiceUrlConventions.KeyAsSegment;
            config.SetEntitySetPageSize("*", 100);
            config.SetEntitySetPageSize("Customers", 20);
            config.SetEntitySetPageSize("Products", 20);
            config.SetEntitySetPageSize("Invoices", 500);
            config.SetEntitySetPageSize("Orders", 200);
            config.SetEntitySetPageSize("Order_Details", 500);
            config.UseVerboseErrors = true;
        }

        /// <summary>
        /// OnStartProcessingRequest is overriden to get Caching support
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);

            int cacheTimeout;
            if (Int32.TryParse(ConfigurationManager.AppSettings["cacheTimeout"], out cacheTimeout) && cacheTimeout > 0)
            {
                HttpContext context = HttpContext.Current;
                HttpCachePolicy c = HttpContext.Current.Response.Cache;
                c.SetCacheability(HttpCacheability.ServerAndPrivate);
                c.SetExpires(HttpContext.Current.Timestamp.AddSeconds(cacheTimeout));
                c.VaryByHeaders["Accept"] = true;
                c.VaryByHeaders["Accept-Charset"] = true;
                c.VaryByHeaders["Accept-Encoding"] = true;
                c.VaryByParams["*"] = true;
            }
        }
    }
}
