using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData;

namespace ODataSamples.CustomFormatService
{
    public class PeopleController : ODataController
    {
        private CustomFormatContext context = CustomFormatContext.Instance;

        public IHttpActionResult Get()
        {
            return Ok(context.People.AsQueryable());
        }

        public IHttpActionResult Get(int key)
        {
            var person = this.context.People.SingleOrDefault(p => p.Id == key);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        public IHttpActionResult Post(Person person)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            return Created(this.context.AddPerson(person));
        }

        public IHttpActionResult GetCard(int key)
        {
            var person = this.context.People.SingleOrDefault(p => p.Id == key);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person.Card);
        }
    }
}