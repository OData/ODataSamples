using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lab01Sample01.Models
{
    [DataContract]
    public class Author
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "authorName")]
        public string AuthorName { get; set; }

        [DataMember(Name = "addresses")]
        public IList<Address> Addresses { get; set; }
    }
}
