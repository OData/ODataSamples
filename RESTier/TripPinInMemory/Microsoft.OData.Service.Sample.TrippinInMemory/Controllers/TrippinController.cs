using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Microsoft.OData.Service.Sample.TrippinInMemory.Models;

namespace Microsoft.OData.Service.Sample.TrippinInMemory.Controllers
{
    /// <summary>
    /// Functions/Actions don't work with Restier 0.5.0-beta now,
    /// Use this controller to handle functions/actions;
    /// </summary>
    public class TrippinController : ODataController
    {
        private TrippinApi api = null;
        private TrippinApi Api
        {
            get {
                if (api == null)
                {
                    api = new TrippinApi();
                }
                return api;
            }
        }

        /// <summary>
        /// Unbound function to get the person who has most number of friends
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ODataRoute("GetPersonWithMostFriends")]
        public IHttpActionResult GetPersonWithMostFriends()
        {
            return Ok(Api.GetPersonWithMostFriends());
        }

        /// <summary>
        /// Action to update the LastName of one person,
        /// Notice that the parameter name should be {"value":"test"} rather than {"name":"test"}
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPut]
        [ODataRoute("People({key})/LastName")]
        public IHttpActionResult UpdatePersonLastName([FromODataUri]string key, [FromBody] string name)
        {
            System.Diagnostics.Debug.WriteLine("Input LastName is: " + name);
            if (Api.UpdatePersonLastName(key, name))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}