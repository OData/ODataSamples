using System.Collections.Generic;

namespace MessageApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> DynamicProperties { get; set; }
    }
}
