using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoundActionSampleClient.ServiceReference;

namespace BoundActionSampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var contextAtom = new MyContainer(new Uri("http://localhost:39401/odata/"));
            contextAtom.Format.UseAtom();
            var myEntityAtom = contextAtom.MyEntities.First();

            // Outputs: http://localhost:39401/odata/MyEntities(guid'2c2431cd-4afa-422b-805b-8398b9a29fec')/MyAction
            var uriAtom = contextAtom.GetEntityDescriptor(myEntityAtom).OperationDescriptors.First().Target;
            Console.WriteLine(uriAtom);

            // Works fine using ATOM format!
            var responseAtom = contextAtom.Execute<string>(uriAtom, "POST", true);

            var contextJson = new MyContainer(new Uri("http://localhost:39401/odata/"));
            contextJson.Format.UseJson();
            var myEntityJson = contextJson.MyEntities.First();

            // Outputs: http://localhost:39401/odata/MyEntities(guid'f31a8332-025b-4dc9-9bd1-27437ae7966a')/MyContainer.MyAction
            var uriJson = contextJson.GetEntityDescriptor(myEntityJson).OperationDescriptors.First().Target;
            Console.WriteLine(uriJson);

            // Throws an exception using the JSON uri in JSON format!
            var responseJson = contextJson.Execute<string>(uriJson, "POST", true);

            // Works fine using ATOM uri in JSON format!
            var responseJson2 = contextJson.Execute<string>(uriAtom, "POST", true);
        }
    }
}
