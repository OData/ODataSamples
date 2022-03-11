using System.Collections.Generic;

namespace ODataAspnetCore7xSample.Models
{
    public class Author
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
