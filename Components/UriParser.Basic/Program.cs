using System;
using Microsoft.OData.Core.UriParser;
using ODataSamples.Common.Model;
using ODataSamples.Common.Extensions;

namespace ParseBasic
{
    class Program
    {
        private static readonly Uri ServiceRoot = new Uri("http://demo/odata.svc/");
        private static ParserExtModel extModel = new ParserExtModel();

        static void Main(string[] args)
        {
            KeyContainingSpecialChar();
            KeyAfterFunction();
            FilterOnOpenProperty();
        }

        private static void KeyContainingSpecialChar()
        {
            Console.WriteLine("KeyContainingSpecialChar");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/Resources('w%23j')"));

            var path = parser.ParsePath();
            Console.WriteLine(path.ToLogString());
        }

        private static void FilterOnOpenProperty()
        {
            Console.WriteLine("FilterOnOpenProperty");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/Resources?$filter=Name eq 'w'"));

            var filter = parser.ParseFilter();
            Console.WriteLine(filter.Expression.ToLogString());
        }

        private static void KeyAfterFunction()
        {
            Console.WriteLine("TestKeyAfterFunction");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People(2)/TestNS.GetFriends(2)"));

            var path=parser.ParsePath();
            Console.WriteLine(path.ToLogString());
        }
    }
}
