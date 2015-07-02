using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using CapabilitiesVocabulary;

namespace ODataCapabilitiesVocabularySample.Controllers
{
    public class CustomersController : ODataController
    {
        private static IList<Customer> _customers;

        static CustomersController()
        {
            const int count = 7;

            IList<IList<Color>> colors = new IList<Color>[]
            {
                new[] {Color.Red, Color.Green},
                new[] {Color.Blue, Color.Blue},
                new[] {Color.Pink, Color.Yellow, Color.Purple},
                new[] {Color.Green, Color.Green},
                new[] {Color.Purple, Color.Blue},
                new[] {Color.Yellow, Color.Purple},
                new[] {Color.Red, Color.Purple, Color.Green},
            };

            Guid[] tokens =
            {
                new Guid("F83E70FC-CFAB-45EF-9056-FB3D9B71E221"),
                new Guid("7548E3A2-6BB2-4797-92C6-11008A8FFDD3"),
                new Guid("F0B68809-7E49-4447-820F-6533A4E8EAF9"),
                new Guid("BD4CDC8E-45AB-4F20-86C5-6EE838C8D554"),
                new Guid("1E72CC05-406C-40FA-ACF9-AD24D87B74E1"),
                new Guid("E9ACEEB0-DE5B-42B1-A4E9-B1B9CF71994F"),
                new Guid("57B7C8E1-10BF-4EB0-9BD3-A54A51530E30")
            };

            string[] names = {"John", "Peter", "Mike", "Sam", "Mark", "Ted", "Bear"};

            Address redmond = new Address {City = "Redmond", Street = "One Microsoft way"};
            Address shanghai = new Address {City = "Shanghai", Street = "ZiXing Rd"};
            Address beijing = new Address {City = "Beijing", Street = "Fujian Rd"};

            IList<IList<Address>> addresses = new IList<Address>[]
            {
                new []{ redmond },
                new []{ shanghai },
                new []{ beijing },
                new []{ redmond, beijing },
                new []{ redmond, beijing, shanghai },
                new []{ beijing, shanghai },
                new []{ redmond, shanghai }
            };

            _customers = Enumerable.Range(1, count).Select(e =>
            new Customer
            {
                CustomerId = e,
                Name = names[e - 1],
                FavoriateColors = colors[e - 1],
                Addresses = addresses[e - 1],
                Email = names[e -1] + "@microsoft.com",
                Token = tokens[e -1],
                Orders = Enumerable.Range(1, e).Select(f => new Order
                {
                    OrderId = 10 * e + f,
                    Price = 9.9 *e + f
                }).ToList()
            }).ToList();
        }

        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(_customers);
        }
    }
}
