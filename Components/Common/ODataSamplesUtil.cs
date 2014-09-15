using System.IO;

namespace ODataSamples.Common
{
    public static class ODataSamplesUtil
    {
        public static Message CreateMessage()
        {
            return new Message { Stream = new MemoryStream() };
        }

        public static string MessageToString(Message message)
        {
            message.Stream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(message.Stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
