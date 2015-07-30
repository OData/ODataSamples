using System;
using System.Collections.Generic;
using Microsoft.OData.Core;
using ODataSamples.Common;
using ODataSamples.Common.Model;

namespace ODataSamples.Writer
{
    class Program
    {
        private static readonly Uri ServiceRoot = new Uri("http://demo/odata.svc/");
        private static readonly ParserExtModel ExtModel = new ParserExtModel();
        private static readonly ODataFeed Feed;
        private static readonly ODataComplexValue Address1;
        private static readonly ODataEntry PersonEntry;
        private static readonly ODataEntry PetEntry;
        private static readonly ODataEntry FishEntry;
        private static readonly ODataNavigationLink PersonFavouritePetNavigationLink;
        private static readonly ODataNavigationLink PersonPetsNavigationLink;
        private static readonly ODataMessageWriterSettings BaseSettings = new ODataMessageWriterSettings()
        {
            ODataUri = new ODataUri { ServiceRoot = ServiceRoot },
            DisableMessageStreamDisposal = true,
            Indent = true,
        };

        static Program()
        {
            #region Feed and entry definition
            Feed = new ODataFeed();

            Address1 = new ODataComplexValue()
            {
                InstanceAnnotations = new List<ODataInstanceAnnotation>()
                {
                    new ODataInstanceAnnotation("ns.ann2", new ODataPrimitiveValue("hi"))
                },
                TypeName = "TestNS.Address", // Need this for parsed model.
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "ZipCode",
                        Value = "200",
                    },
                },
            };

            #region PersonEntry
            PersonEntry = new ODataEntry()
            {
                InstanceAnnotations = new List<ODataInstanceAnnotation>()
                {
                    new ODataInstanceAnnotation("ns.ann1", new ODataPrimitiveValue("hi"))
                },
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 1,
                    },
                    new ODataProperty()
                    {
                        Name = "Name",
                        Value = "Shang",
                    },
                    new ODataProperty()
                    {
                        Name = "Addr",
                        Value = Address1
                    }
                },
            };
            #endregion

            #region PetEntry
            PetEntry = new ODataEntry()
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 1,
                    },
                    new ODataProperty()
                    {
                        Name = "Color",
                        Value = new ODataEnumValue("Cyan")
                    },
                },
            };
            #endregion PetEntry

            FishEntry = new ODataEntry()
            {
                TypeName = "TestNS.Fish",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty()
                    {
                        Name = "Id",
                        Value = 2,
                    },
                    new ODataProperty()
                    {
                        Name = "Color",
                        Value = new ODataEnumValue("Blue"),
                    },
                    new ODataProperty()
                    {
                        Name = "Name",
                        Value = "Qin",
                    },
                },
            };
            #endregion

            PersonFavouritePetNavigationLink = new ODataNavigationLink
            {
                Name = "FavouritePet",
                AssociationLinkUrl = new Uri("Person(1)/FavouritePetPet/$ref", UriKind.Relative),
                Url = new Uri("Person(1)/FavouritePetPet", UriKind.Relative),
                IsCollection = false
            };

            PersonPetsNavigationLink = new ODataNavigationLink
            {
                Name = "Pets",
                AssociationLinkUrl = new Uri("Person(1)/Pets/$ref", UriKind.Relative),
                Url = new Uri("Person(1)/Pets", UriKind.Relative),
                IsCollection = true
            };
        }

        static void Main(string[] args)
        {
            WriteTopLevelFeed();
            WriteTopLevelEntry();
            ContainmentTest.FeedWriteReadNormal();
            WriteTopLevelEntityReferenceLinks();
            WriteInnerEntityReferenceLink();
        }

        private static void WriteTopLevelFeed(bool enableFullValidation = true)
        {
            Console.WriteLine("WriteTopLevelFeed, enableFullValidation:{0}", enableFullValidation);

            var msg = ODataSamplesUtil.CreateMessage();

            var settings = new ODataMessageWriterSettings(BaseSettings)
            {
                EnableFullValidation = enableFullValidation
            };

            using (var omw = new ODataMessageWriter((IODataResponseMessage)msg, settings, ExtModel.Model))
            {
                var writer = omw.CreateODataFeedWriter(ExtModel.PetSet);
                writer.WriteStart(Feed);
                writer.WriteStart(PetEntry);
                writer.WriteEnd();
                writer.WriteStart(FishEntry);
                writer.WriteEnd();
                writer.WriteEnd();
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }

        private static void WriteTopLevelEntry()
        {
            Console.WriteLine("WriteTopLevelEntry");

            var msg = ODataSamplesUtil.CreateMessage();
            msg.PreferenceAppliedHeader().AnnotationFilter = "*";

            using (var omw = new ODataMessageWriter((IODataResponseMessage)msg, BaseSettings, ExtModel.Model))
            {
                var writer = omw.CreateODataEntryWriter(ExtModel.People);
                writer.WriteStart(PersonEntry);

                writer.WriteStart(PersonPetsNavigationLink);
                writer.WriteStart(new ODataFeed());
                writer.WriteStart(PetEntry);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();

                writer.WriteStart(PersonFavouritePetNavigationLink);
                writer.WriteStart(PetEntry);
                writer.WriteEnd();
                writer.WriteEnd();

                writer.WriteEnd();
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }

        private static void WriteTopLevelEntityReferenceLinks()
        {
            Console.WriteLine("WriteTopLevelEntityReferenceLinks");

            var msg = ODataSamplesUtil.CreateMessage();
            msg.PreferenceAppliedHeader().AnnotationFilter = "*";

            var link1 = new ODataEntityReferenceLink() { Url = new Uri("http://demo/odata.svc/People(3)") };
            var link2 = new ODataEntityReferenceLink() { Url = new Uri("http://demo/odata.svc/People(4)") };

            var links = new ODataEntityReferenceLinks()
            {
                Links = new[] { link1, link2 }
            };

            using (var omw = new ODataMessageWriter((IODataResponseMessage)msg, BaseSettings, ExtModel.Model))
            {
                omw.WriteEntityReferenceLinks(links);
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }

        private static void WriteInnerEntityReferenceLink()
        {
            Console.WriteLine("WriteInnerEntityReferenceLink in Request Payload (odata.bind)");

            var msg = ODataSamplesUtil.CreateMessage();
            msg.PreferenceAppliedHeader().AnnotationFilter = "*";

            var link1 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://demo/odata.svc/PetSet(Id=1,Color=TestNS.Color'Blue')")
            };
            var link2 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://demo/odata.svc/PetSet(Id=2,Color=TestNS.Color'Blue')")
            };

            using (var omw = new ODataMessageWriter((IODataRequestMessage)msg, BaseSettings, ExtModel.Model))
            {
                var writer = omw.CreateODataEntryWriter(ExtModel.People);
                writer.WriteStart(PersonEntry);

                writer.WriteStart(PersonPetsNavigationLink);
                writer.WriteEntityReferenceLink(link1);
                writer.WriteEntityReferenceLink(link2);
                writer.WriteEnd();

                writer.WriteStart(PersonFavouritePetNavigationLink);
                writer.WriteEntityReferenceLink(link1);
                writer.WriteEnd();

                writer.WriteEnd();
            }

            Console.WriteLine(ODataSamplesUtil.MessageToString(msg));
        }
    }
}
