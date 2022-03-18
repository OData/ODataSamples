using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Lab01Sample01.Models;

namespace Lab01Sample01.Controllers
{
    public class BooksController : ODataController
    {
        #region CRUD operations
        // Get ~/Books
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(DataSource.Instance.Books);
        }

        // GET ~/Books(1)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT ~/Books(1)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = DataSource.Instance.Books.Where(b => b.ID == key).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PATCH ~/Books(1)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Book> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // DELETE ~/Books(1)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }
        #endregion

        #region Non-Contained Navigation
        [EnableQuery]
        public IActionResult GetMainAuthor([FromODataUri] int key)
        {
            var mainAuthor = DataSource.Instance.Books.Where(m => m.ID == key).Select(m => m.MainAuthor).FirstOrDefault();

            if (mainAuthor == null)
            {
                return NotFound();
            }

            return Ok(mainAuthor);
        }

        // GET ~/Books(1)/Authors
        // Authors is a navigation property.
        [EnableQuery]
        public IActionResult GetAuthors([FromODataUri] int key)
        {
            var authors = DataSource.Instance.Books.AsQueryable<Book>().Where(b => b.ID == key).Select(b => b.Authors);
            return Ok(authors);
        }
        #endregion

        #region Contained Navigation
        //Contained entities don't have their own controller; the action is defined in the containing entity set controller.

        // GET ~/Books(1)/Translators
        [EnableQuery]
        public IActionResult GetTranslators(int key)
        {
            var translators = DataSource.Instance.Books.Single(b => b.ID == key).Translators;
            return Ok(translators);
        }

        // GET ~/Books(1)/Translators(100001)
        [EnableQuery]
        [ODataRoute("Books({bookId})/Translators({TranslatorID})")]
        public IActionResult GetSingleTranslator(int bookId, int translatorId)
        {
            var translators = DataSource.Instance.Books.FirstOrDefault(b => b.ID == bookId).Translators;
            var translator = translators.FirstOrDefault(t => t.TranslatorID == translatorId);

            if (translator == null)
            {
                return NotFound();
            }

            return Ok(translator);
        }

        // PUT ~/Books(1)/Translators(100001)
        [ODataRoute("Books({bookId})/Translators({TranslatorID})")]
        public IActionResult PutToTranslator(int bookId, int TranslatorID, [FromBody] Translator translator)
        {
            var book = DataSource.Instance.Books.Single(b => b.ID == bookId);
            var originalTranslator = book.Translators.Single(t => t.TranslatorID == TranslatorID);
            originalTranslator.TranslatorName = translator.TranslatorName;
            return Ok(translator);
        }

        // DELETE ~/Books(1)/Translators(100001)
        [ODataRoute("Books({bookId})/Translators({TranslatorID})")]
        public IActionResult DeleteTranslatorFromBook(int bookId, int TranslatorID)
        {
            var book = DataSource.Instance.Books.Single(b => b.ID == bookId);
            var originalTranslator = book.Translators.Single(t => t.TranslatorID == TranslatorID);
            if (book.Translators.Remove(originalTranslator))
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion
    }
}