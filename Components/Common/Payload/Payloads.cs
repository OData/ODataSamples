using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.Common.Payload
{
    public static class Payloads
    {
        public static Stream GetStreamFromResource(string resourceName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Failed to get stream with name '{0}', available resource names:", resourceName);
                foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                {
                    builder.AppendLine(name);
                }

                throw new ApplicationException(builder.ToString());
            }

            return stream;
        }
    }
}
