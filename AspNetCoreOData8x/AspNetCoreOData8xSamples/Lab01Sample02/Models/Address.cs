using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Lab01Sample02.Models
{
    [ComplexType]
    [DataContract(Name = "address")]
    public class Address
    {
        [DataMember(Name = "town")]
        public string Town { get; set; }

        [DataMember(Name = "county")]
        public string County { get; set; }
    }
}
