using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData.Formatter.Serialization;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace ODataSamples.CustomFormatService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var vcardFormatter = new CustomFormatter(
                new DefaultODataDeserializerProvider(),
                new DefaultODataSerializerProvider(),
                ODataPayloadKind.Property);
            vcardFormatter.SupportedEncodings.Add(Encoding.UTF8);
            vcardFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/x-vCard"));

            var csvFormatter = new CustomFormatter(
                new DefaultODataDeserializerProvider(),
                new DefaultODataSerializerProvider(),
                ODataPayloadKind.Entry,
                ODataPayloadKind.Feed);
            csvFormatter.SupportedEncodings.Add(Encoding.UTF8);
            csvFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));

            config.Formatters.Insert(0, vcardFormatter);
            config.Formatters.Insert(0, csvFormatter);
            config.Formatters.InsertRange(0, ODataMediaTypeFormatters.Create());

            config.MapODataServiceRoute("CustomFormatService", null, GetModel());
        }

        private static IEdmModel GetModel()
        {
            var builder = new ODataConventionModelBuilder { Namespace = "ODataSamples.CustomFormatService" };
            builder.EntitySet<Person>("People");
            return builder.GetEdmModel();
        }
    }
}
