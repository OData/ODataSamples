using System.ComponentModel.DataAnnotations.Schema;

namespace ODataAspnetCore7xSample.Models
{
    [ComplexType]
    public class Address
    {
        public string Town { get; set; }
        public string County { get; set; }
    }
}
