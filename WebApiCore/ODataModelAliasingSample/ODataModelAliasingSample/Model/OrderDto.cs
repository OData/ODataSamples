
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataModelAliasingSample.AspNetCore.Model
{
    // The name is changed to Order in the IEdmModel when we build it using the ODataModelBuilder.
    public class OrderDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        // The name is changed to Check in the IEdmModel when we build it using the ODataModelBuilder.
        public double Total { get; set; }
    }
}
