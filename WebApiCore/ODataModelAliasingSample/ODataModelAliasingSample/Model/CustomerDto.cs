using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ODataModelAliasingSample.AspNetCore.Model
{
    [DataContract(Name = "Customer")]
    public class CustomerDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        [DataMember(Name = "FirstName")]
        public string GivenName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember(Name = "Orders")]
        public virtual ICollection<OrderDto> Purchases { get; set; }
    }
}
