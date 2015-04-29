using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace EdmLibSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new SampleModelBuilder();
            var model = builder.GetModel();
            var csdl = WriteModelToCsdl(model);
            Console.WriteLine(csdl);
        }

        private static string WriteModelToCsdl(IEdmModel model)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(stringBuilder))
            {
                IEnumerable<EdmError> errors;
                if (!model.TryWriteCsdl(writer, out errors))
                {
                    return null;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
