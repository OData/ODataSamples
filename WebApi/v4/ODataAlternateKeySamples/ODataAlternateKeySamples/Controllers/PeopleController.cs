using System;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using ODataAlternateKeySamples.Models;

namespace ODataAlternateKeySamples.Controllers
{
    public class PeopleController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(AlternateKeysDataSource.People);
        }

        public IHttpActionResult Get(int key)
        {
            foreach (var person in AlternateKeysDataSource.People)
            {
                object value;
                if (person.TryGetPropertyValue("ID", out value))
                {
                    int intKey = (int)value;
                    if (key == intKey)
                    {
                        return Ok(person);
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [ODataRoute("People(Country={country},Passport={passport})")]
        public IHttpActionResult FindPeopleByCountryAndPassport([FromODataUri]string country, [FromODataUri]string passport)
        {
            foreach (var person in AlternateKeysDataSource.People)
            {
                object value;
                if (person.TryGetPropertyValue("Country", out value))
                {
                    string countryValue = (string)value;
                    if (person.TryGetPropertyValue("Passport", out value))
                    {
                        string passportValue = (string)value;
                        if (countryValue == country && passportValue == passport)
                        {
                            return Ok(person);
                        }
                    }
                }
            }

            return NotFound();
        }
    }
}