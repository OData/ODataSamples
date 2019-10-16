using System;
using ODataService.Models;
using Simple.OData.Client;

namespace ODataService.SimpleODataClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ODataClient("http://localhost:2947/odata/");

            Console.WriteLine("Welcome to the OData Web Api command line client sample.");
            Console.WriteLine("\tType '?' for options.");

            while (true)
            {
                Console.Write("> ");
                string commmand = Console.ReadLine().ToLower();
                switch (commmand)
                {
                    case "get products":
                        GetProducts(client);
                        break;

                    case "query products":
                        QueryProducts(client);
                        break;

                    case "get productfamilies":
                        GetProductFamilies(client);
                        break;

                    case "?":
                    case "h":
                    case "help":
                        PrintOptions();
                        break;

                    case "q":
                    case "quit":
                        return;
                    default:
                        HandleUnknownCommand();
                        break;

                }
            }
        }

        private static void GetProducts(ODataClient client)
        {
            Console.WriteLine("\n\t<< get products >>");

            var products = client.FindEntriesAsync("Products").Result;

            foreach (var product in products)
            {
                Console.WriteLine("{");
                foreach (var property in product)
                {
                    Console.WriteLine($"\t{property.Key}: {property.Value}" );
                }
                Console.WriteLine("}");
            }
        }

        private static void QueryProducts(ODataClient client)
        {

            Console.WriteLine("\n\t<< query products >>");
            Console.WriteLine("\n\tGet top 4 products");

            var products = client.For("Products").Top(4).FindEntriesAsync().Result;
            foreach (var product in products)
            {
                Console.WriteLine("{");
                foreach (var property in product)
                {
                    Console.WriteLine($"\t{property.Key}: {property.Value}");
                }
                Console.WriteLine("}");
            }


            Console.WriteLine("\n\tGet products with name starting with 'Microsoft Office'");

            products = client.For("Products").Filter("startswith(Name,'Microsoft Office')").FindEntriesAsync().Result;

            foreach (var product in products)
            {
                Console.WriteLine("{");
                foreach (var property in product)
                {
                    Console.WriteLine($"\t{property.Key}: {property.Value}");
                }
                Console.WriteLine("}");
            }
        }

        private static void GetProductFamilies(ODataClient client)
        {
            Console.WriteLine("\n\t<< get productfamilies >>");

            var families = client.For("ProductFamilies").FindEntriesAsync().Result;

            foreach (var productFamily in families)
            {
                Console.WriteLine("{");
                foreach (var property in productFamily)
                {
                    Console.WriteLine($"\t{property.Key}: {property.Value}");
                }
                Console.WriteLine("}");
            }
        }

        private static void PrintOptions()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("\tget products                   -> Print all the Products.");
            Console.WriteLine("\tquery products                 -> Query Products.");
            Console.WriteLine("\tget productfamilies            -> Print all the ProductFamilies.");
            Console.WriteLine("\t?                              -> Print Available Commands.");
            Console.WriteLine("\tquit                           -> Quit.");
        }

        private static void HandleUnknownCommand()
        {
            Console.WriteLine("command not recognized, enter '?' for options");
        }
    }
}
