using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Lab01Sample01.Models;

namespace Lab01Sample01.Controllers
{
    public class AuthorsController : ODataController
    {
        #region CRUD operations
        // Get ~/Authors
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IQueryable<Author> Get()
        {
            return DataSource.Instance.Authors.AsQueryable<Author>();
        }

        // GET ~/Authors(1)
        [EnableQuery]
        public SingleResult<Author> Get(int key)
        {
            IQueryable<Author> result = DataSource.Instance.Authors.AsQueryable<Author>().Where(a => a.ID == key);
            return SingleResult.Create(result);
        }

        // PUT ~/Authors(1)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Author Author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = DataSource.Instance.Authors.Where(a => a.ID == key);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PATCH ~/Authors(1)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Author> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Author = DataSource.Instance.Authors.Where(a => a.ID == key);
            if (Author == null)
            {
                return NotFound();
            }

            return Ok(Author);
        }

        // DELETE ~/Authors(1)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var Author = DataSource.Instance.Authors.Where(a => a.ID == key);
            if (Author == null)
            {
                return NotFound();
            }

            return Ok(Author);
        }
        #endregion
    }
}
