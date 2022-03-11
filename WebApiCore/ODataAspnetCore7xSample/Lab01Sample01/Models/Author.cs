using System.Collections.Generic;

namespace Lab01Sample01.Models
{
    public class Author
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
