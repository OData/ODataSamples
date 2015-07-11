using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using MessageApp.Models;

namespace MessageApp.Controllers
{
    public class MessagesController : ODataController
    {
        // GET: odata/Messages
        public IHttpActionResult Get()
        {
            var msgs = new[] { GetMessage() };
            return Ok(msgs.AsQueryable());
        }

        // GET: odata/Messages(5)
        public IHttpActionResult GetMessage(int key)
        {
            return Ok(GetMessage());
        }

        private static Message GetMessage()
        {
            var ext = new Extension
            {
                DynamicProperties = new Dictionary<string, object>
                {
                    {"EName1", "e value 1"},
                }
            };

            var ext2 = new Extension
            {
                DynamicProperties = new Dictionary<string, object>
                {
                    {"EName2", "e value 2"},
                }
            };

            var ext3 = new Extension
            {
                DynamicProperties = new Dictionary<string, object>
                {
                    {"EName3", "e value 3"},
                    {"EEmbedded1", ext},
                }
            };

            var exts = new[] { ext, ext2, ext3 };

            var dict = new Dictionary<string, object>
            {
                { "DName1", "d value 1" },
                { "Ext1", ext },
                { "Exts", exts },
            };

            var msg = new Message
            {
                Id = 0,
                Name = "abc",
                DynamicProperties = dict,
            };
            return msg;
        }
    }
}
