using System;
using System.Net.Http;

namespace ODataModelAliasingSample.Client
{
    class Program
    {
        private static readonly string serviceUrl = "https://localhost:44327";

        static void Main(string[] args)
        {
            Console.WriteLine("Service url: {0}", serviceUrl);
            RunQueries();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void RunQueries()
        {
            HttpClient client = new HttpClient();

            // Showing the aliased names in the $metadata document.
            // The metadata will display Customer instead of CustomerDto, etc.
            Console.WriteLine("Showing the metadata document with the aliased names");
            Console.WriteLine();
            Console.WriteLine(client.GetStringAsync(serviceUrl + "/odata/$metadata").Result);
            Console.WriteLine();

            // Querying for a customer to see the aliased payload.
            // Check that the payload reflects that GivenName is aliased to FirstName.
            Console.WriteLine("Querying a single customer at /Customers(1):");
            Console.WriteLine();
            Console.WriteLine(client.GetStringAsync(serviceUrl + "/odata/Customers").Result);
            Console.WriteLine();

            // Querying the customers feed and using the aliased property name on a $filter clause.
            // Look at the FirstName property in the query string instead of GivenName as in the CLR object.
            Console.WriteLine("Querying for the feed of customers and filtering on an aliased property:");
            Console.WriteLine();
            Console.WriteLine(client.GetStringAsync(serviceUrl + "/odata/Customers?$filter=FirstName le 'First name 5'").Result);
            Console.WriteLine();

            // Querying the orders of a customer.
            // Look at the Orders property in the path of the URI instead of Purchases on the CLR type name.
            Console.WriteLine("Querying for the orders associated to a customer:");
            Console.WriteLine();
            Console.WriteLine(client.GetStringAsync(serviceUrl + "/odata/Customers(1)/Orders").Result);
        }
    }
}
