using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Lab02Sample03.Models;

namespace Lab02Sample03.Controllers
{
    public class PublishersController : ODataController
    {
        #region CRUD operations
        // Get ~/Publishers
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(DataSource.Instance.Publishers);
        }

        // GET ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // PUT ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Publisher Publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // PATCH ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Publisher> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }

        // DELETE ~/Publishers(1001)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var publisher = DataSource.Instance.Publishers.FirstOrDefault(p => p.ID == key);
            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }
        #endregion
    }
}
