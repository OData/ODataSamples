using System.Collections.Generic;

namespace Lab02Sample03.Models
{
    public class Publisher
    {
        public int ID { get; set; }
        public string PublisherName { get; set; }
        public Address Address { get; set; }
        public IList<Author> Authors { get; set; }
    }
}
