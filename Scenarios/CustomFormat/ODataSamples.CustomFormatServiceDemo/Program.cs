using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.OData.Client;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using ODataSamples.CustomFormatService;
using ODataSamples.CustomFormatService.Formaters.CSV;

namespace ODataSamples.CustomFormatServiceDemo
{
    class Program
    {
        private static readonly Uri serviceRoot = new Uri("http://localhost:23429/");

        static void Main(string[] args)
        {
            CsvWriterDemo();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            CsvServiceDemo();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            VCardServiceDemo();
        }

        private static void CsvWriterDemo()
        {
            EdmEntityType customer = new EdmEntityType("ns", "customer");
            var key = customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            customer.AddKeys(key);
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            ODataEntry entry1 = new ODataEntry()
            {
                Properties = new[]
        {
            new ODataProperty(){Name = "Id", Value = 51}, 
            new ODataProperty(){Name = "Name", Value = "Name_A"}, 
        }
            };

            ODataEntry entry2 = new ODataEntry()
            {
                Properties = new[]
        {
            new ODataProperty(){Name = "Id", Value = 52}, 
            new ODataProperty(){Name = "Name", Value = "Name_B"}, 
        }
            };

            var stream = new MemoryStream();
            var message = new Message { Stream = stream };
            // Set Content-Type header value
            message.SetHeader("Content-Type", "text/csv");
            var settings = new ODataMessageWriterSettings
            {
                // Set our resolver here.
                MediaTypeResolver = CsvMediaTypeResolver.Instance,
                DisableMessageStreamDisposal = true,
            };
            using (var messageWriter = new ODataMessageWriter(message, settings))
            {
                var writer = messageWriter.CreateODataFeedWriter(null, customer);
                writer.WriteStart(new ODataFeed());
                writer.WriteStart(entry1);
                writer.WriteEnd();
                writer.WriteStart(entry2);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            string msg;
            using (var sr = new StreamReader(stream)) { msg = sr.ReadToEnd(); }
            Console.WriteLine(msg);
        }

        private static void CsvServiceDemo()
        {
            Console.WriteLine("Csv demo");
            Console.WriteLine("Add new Person entity");
            var ctx = new Container(serviceRoot);
            var person = new Person() { Comment = "some comment" };
            ctx.AddToPeople(person);
            ctx.SaveChanges();
            Console.WriteLine("New person id is {0}", person.Id);

            string fileName = string.Format("person{0}.csv", person.Id);
            Console.WriteLine("Download the csv file {0}.", fileName);
            DownloadFile(ctx.People.ByKey(person.Id).GetPath(""), fileName, "text/csv");

            Console.WriteLine("Open the file");
            Process.Start(fileName);
        }

        private static void VCardServiceDemo()
        {
            var ctx = new Container(serviceRoot);
            var person = new Person()
            {
                Card = new BusinessCard { ORG = "New Org", N = "LN2;FN1", FN = "LF2", Title = "New Title" }
            };

            ctx.AddToPeople(person);
            ctx.SaveChanges();
            Console.WriteLine("New person id is {0}", person.Id);

            string fileName = string.Format("person{0}_vcard.vcf", person.Id);
            Console.WriteLine("Download the csv file {0}.", fileName);
            DownloadFile(ctx.People.ByKey(person.Id).GetPath("Card"), fileName, "text/x-vCard");

            Console.WriteLine("Open the file");
            Process.Start(fileName);
        }

        private static void DownloadFile(string path, string filename, string mediaType)
        {
            Console.WriteLine("Request path is {0}", path);
            byte[] data;
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Accept", mediaType);
                // Skip the UTF-8 BOM header
                data = webClient.DownloadData(new Uri(serviceRoot, path)).ToArray();
            }

            // Write to tmp file.
            File.WriteAllBytes(filename, data);
        }
    }
}
