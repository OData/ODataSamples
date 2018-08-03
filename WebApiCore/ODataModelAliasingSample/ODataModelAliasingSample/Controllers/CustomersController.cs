using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODataModelAliasingSample.AspNetCore.Model;

namespace ODataModelAliasingSample.Controllers
{
    public class CustomersController : ODataController
    {
        private static CustomersContext _context;

        public CustomersController(CustomersContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Customers);
        }

        [EnableQuery]
        public IActionResult GetOrders(int key)
        {
            CustomerDto customer = _context.Customers.Include(c => c.Purchases).SingleOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer.Purchases);
        }
    }
}
