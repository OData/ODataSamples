using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.OData.Builder;
using Microsoft.OData.Edm;

namespace ODataReferentialConstraintSample
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public string OrderName { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        [ActionOnDelete(EdmOnDeleteAction.Cascade)]
        public Customer Customer { get; set; }
    }
}
