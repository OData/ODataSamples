using System.ComponentModel.DataAnnotations.Schema;

namespace Lab02Sample03.Models
{
    [ComplexType]
    public class Address
    {
        public string Town { get; set; }
        public string County { get; set; }
    }
}
