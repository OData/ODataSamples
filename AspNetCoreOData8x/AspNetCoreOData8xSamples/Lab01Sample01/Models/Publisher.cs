using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lab01Sample01.Models
{
    [DataContract(Name = "publisher")]
    public class Publisher
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "publisherName")]
        public string PublisherName { get; set; }

        [DataMember(Name = "address")]
        public Address Address { get; set; }

        [DataMember(Name = "authors")]
        public IList<Author> Authors { get; set; }
    }
}
