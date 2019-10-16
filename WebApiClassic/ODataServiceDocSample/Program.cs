using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace GenerateServiceDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = (args.Length == 0) ? "trippin-csdl.xml" : args[0];
            var edmModel = LoadEdmModel(filePath);
            ODataServiceDocument serviceDoc = new ODataServiceDocument();
            serviceDoc = edmModel.GenerateServiceDocument();
            var messageWriterSettings = new ODataMessageWriterSettings();
            messageWriterSettings.ODataUri.ServiceRoot = new Uri("http://microsoft.com");
            var message = new InMemoryMessage() { Stream = new MemoryStream() };
            ODataMessageWriter writer = new ODataMessageWriter((IODataResponseMessage)message, messageWriterSettings);
            writer.WriteServiceDocument(serviceDoc);
            message.Stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(message.Stream);
            File.WriteAllText(filePath.Replace(".xml", "-service-doc.xml"), reader.ReadToEnd());
            Console.Write("Service doc created");
            Console.ReadLine();
        }

        private static IEdmModel LoadEdmModel(string file)
        {
            try
            {
                string csdl = File.ReadAllText(file);
                Console.WriteLine("Metadata loaded successfully");
                return CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            }
            catch
            {
                Console.WriteLine("Cannot load metadata from " + file);
                return null;
            }
        }
    }
    public class InMemoryMessage : IODataRequestMessage, IODataResponseMessage, IContainerProvider, IDisposable
    {
        private readonly Dictionary<string, string> headers;
        public InMemoryMessage()
        {
            headers = new Dictionary<string, string>();
        }
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return headers; }
        }

        public int StatusCode { get; set; }
        public Uri Url { get; set; }
        public string Method { get; set; }
        public Stream Stream { get; set; }
        public IServiceProvider Container { get; set; }
        public string GetHeader(string headerName)
        {
            string headerValue;
            return headers.TryGetValue(headerName, out headerValue) ? headerValue : null;
        }
        public void SetHeader(string headerName, string headerValue)
        {
            headers[headerName] = headerValue;
        }
        public Stream GetStream()
        {
            return Stream;
        }
        public Action DisposeAction { get; set; }
        void IDisposable.Dispose()
        {
            DisposeAction?.Invoke();
        }
    }
}