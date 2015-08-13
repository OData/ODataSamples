using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using ODataAlternateKeySamples.Models;

namespace ODataAlternateKeySamples
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableAlternateKeys(true);
            config.MapODataServiceRoute("odata", "odata", AlternateKeyEdmModel.GetEdmModel());
        }
    }
}
