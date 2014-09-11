using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo1;
using Microsoft.OData.Core;
using ODataSamples.Common.Model;

namespace ODataSamples.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            var extModel = new ParserExtModel();
            var stream = new MemoryStream();
            var msg = new Message()
            {
                Stream = stream,
            };

            var settings = new ODataMessageWriterSettings()
            {
                ODataUri = new ODataUri()
                {
                    ServiceRoot = new Uri("http://host/svc")
                },
                DisableMessageStreamDisposal = true,
                JsonPCallback = "fsjflsajflsjalfjas",
            };

            ODataFeed feed = new ODataFeed();
            ODataEntry entry = new ODataEntry();
            entry.TypeName = "TestNS.Fish";
            var ppr = new List<ODataProperty>();
            ppr.Add(new ODataProperty()
            {
                Name = "Color",
                Value = new ODataEnumValue("blue")
            });
            entry.Properties = ppr;

            using (ODataMessageWriter omw = new ODataMessageWriter((IODataResponseMessage)msg, settings, extModel.Model))
            {
                var writer = omw.CreateODataFeedWriter(extModel.People);
                writer.WriteStart(feed);
                writer.WriteStart(entry);
                writer.WriteEnd();
                writer.WriteEnd();
            }

            stream.Seek(0, SeekOrigin.Begin);

            using (var sr = new StreamReader(stream))
            {
                var payload = sr.ReadToEnd();
                Console.WriteLine(payload);
            }
        }
    }
}
