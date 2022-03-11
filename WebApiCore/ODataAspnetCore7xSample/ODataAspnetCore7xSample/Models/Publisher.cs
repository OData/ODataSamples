using System.Collections.Generic;

namespace ODataAspnetCore7xSample.Models
{
    public class Publisher
    {
        public int ID { get; set; }
        public string PublisherName { get; set; }
        public Address Address { get; set; }
        public IList<Author> Authors { get; set; }
    }
}
