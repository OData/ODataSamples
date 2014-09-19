using System;
using Microsoft.OData.Core.UriParser;
using ODataSamples.Common.Model;

namespace ParseBasic
{
    class Program
    {
        private static readonly Uri ServiceRoot = new Uri("http://demo/odata.svc/");
        private static ParserExtModel extModel = new ParserExtModel();

        static void Main(string[] args)
        {
            KeyAfterFunction();
        }

        private static void KeyAfterFunction()
        {
            Console.WriteLine("TestKeyAfterFunction");
            var parser = new ODataUriParser(
                extModel.Model,
                ServiceRoot,
                new Uri("http://demo/odata.svc/People(2)/TestNS.GetFriends()(2)"));

            var path=parser.ParsePath();
        }
    }
}
