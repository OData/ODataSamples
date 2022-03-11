using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Lab01Sample02.Models;

namespace Lab01Sample02.Controllers
{
    public class PublishersController : ODataController
    {
        #region CRUD operations
        // Get ~/Publishers
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IQueryable<Publisher> Get()
        {
            return DataSource.Instance.Publishers.AsQueryable<Publisher>();
        }

        // GET ~/Publishers(1)
        [EnableQuery]
        public SingleResult<Publisher> Get(int key)
        {
            IQueryable<Publisher> result = DataSource.Instance.Publishers.AsQueryable<Publisher>().Where(p => p.ID == key);
            return SingleResult.Create(result);
        }

        // PUT ~/Publishers(1)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Publisher Publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = DataSource.Instance.Publishers.Where(p => p.ID == key);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PATCH ~/Publishers(1)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Publisher> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Publisher = DataSource.Instance.Publishers.Where(p => p.ID == key);
            if (Publisher == null)
            {
                return NotFound();
            }

            return Ok(Publisher);
        }

        // DELETE ~/Publishers(1)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var Publisher = DataSource.Instance.Publishers.Where(p => p.ID == key);
            if (Publisher == null)
            {
                return NotFound();
            }

            return Ok(Publisher);
        }
        #endregion
    }
}
