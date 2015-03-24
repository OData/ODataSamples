using System;
using System.IO;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using ODataSamples.Common;
using ODataSamples.Common.Model;

namespace ODataSamples.Writer
{
    public static class ContainmentTest
    {
        public static CraftModel CraftModel = new CraftModel();


        public static void FeedWriteReadNormal()
        {
            Console.WriteLine("FeedWriteReadNormal");

            ODataFeed Feed = new ODataFeed();

            Feed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceName = "Mails",
                NavigationSourceEntityTypeName = "NS.Mail",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });

            ODataEntry Entry = new ODataEntry()
            {
                Properties = new[]
                {
                    new ODataProperty() {Name = "Id", Value = 2},
                },
                EditLink = new Uri("http://example/Web/Users(3)"),
            };

            Entry.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceName = "MyLogin",
                NavigationSourceEntityTypeName = "NS.Person",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });

            // Parse the full request Uri
            ODataPath path = new ODataUriParser(
                CraftModel.Model,
                new Uri("http://example.org/svc/"),
                new Uri("http://example.org/svc/MyLogin/Mails")).ParsePath();

            // Alternatively, construct the normal path for the contained entity manually.
            //ODataPath path = new ODataPath(
            //    new ODataPathSegment[]
            //    {
            //        new SingletonSegment(CraftModel.MyLogin), new NavigationPropertySegment(CraftModel.MailBox, CraftModel.MyLogin)
            //    });

            var stream = new MemoryStream();

            var wsetting = new ODataMessageWriterSettings()
            {
                DisableMessageStreamDisposal = true,

                ODataUri = new ODataUri()
                {
                    ServiceRoot = new Uri("http://example.org/svc/"),
                    Path = path
                }
            };
            IODataResponseMessage msg = new Message()
            {
                Stream = stream,
            };

            var omw = new ODataMessageWriter(msg, wsetting);
            var writer = omw.CreateODataFeedWriter();
            writer.WriteStart(Feed);
            writer.WriteStart(Entry);
            writer.WriteEnd();
            writer.WriteEnd();

            stream.Seek(0, SeekOrigin.Begin);
            var payload = new StreamReader(stream).ReadToEnd();

            // {"@odata.context":"http://example.org/svc/$metadata#Web/Items","value":[{"@odata.editLink":"http://example/Web/Users(3)","Id":2}]}
            Console.WriteLine(payload);

            //Read
            ODataEntry entry = null;
            stream.Seek(0, SeekOrigin.Begin);
            var rsetting = new ODataMessageReaderSettings();
            var omr = new ODataMessageReader(msg, rsetting, CraftModel.Model);
            var reader = omr.CreateODataFeedReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.EntryEnd)
                {
                    entry = (ODataEntry)reader.Item;
                    break;
                }
            }

            //Id=2
            foreach (var prop in entry.Properties)
            {
                Console.WriteLine("{0}={1}", prop.Name, prop.Value);
            }
        }
    }
}
