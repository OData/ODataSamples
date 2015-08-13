using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using ODataAlternateKeySamples.Models;

namespace ODataAlternateKeySamples.Controllers
{
    public class CustomersController : ODataController
    {
        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(AlternateKeysDataSource.Customers);
        }

        [EnableQuery]
        public IHttpActionResult Get(int key)
        {
            foreach (var customer in AlternateKeysDataSource.Customers)
            {
                object value;
                if (customer.TryGetPropertyValue("ID", out value))
                {
                    int intKey = (int)value;
                    if (key == intKey)
                    {
                        return Ok(customer);
                    }
                }
            }

            return NotFound();
        }

        // alternate key: SSN
        [HttpGet]
        [ODataRoute("Customers(SSN={ssn})")]
        public IHttpActionResult GetCustomerBySSN([FromODataUri]string ssn)
        {
            foreach (var customer in AlternateKeysDataSource.Customers)
            {
                object value;
                if (customer.TryGetPropertyValue("SSN", out value))
                {
                    string stringKey = (string)value;
                    if (ssn == stringKey)
                    {
                        return Ok(customer);
                    }
                }
            }

            return NotFound();
        }

        [HttpPatch]
        [ODataRoute("Customers(SSN={ssnKey})")]
        public IHttpActionResult PatchCustomerBySSN([FromODataUri]string ssnKey, EdmEntityObject delta)
        {
            IList<string> changedPropertyNames = delta.GetChangedPropertyNames().ToList();
            IEdmEntityObject originalCustomer = null;
            foreach (var customer in AlternateKeysDataSource.Customers)
            {
                object value;
                if (customer.TryGetPropertyValue("SSN", out value))
                {
                    string stringKey = (string)value;
                    if (ssnKey == stringKey)
                    {
                        originalCustomer = customer;
                    }
                }
            }

            if (originalCustomer == null)
            {
                return NotFound();
            }

            object nameValue;
            delta.TryGetPropertyValue("Name", out nameValue);
            //Assert.NotNull(nameValue);
            //string strName = Assert.IsType<string>(nameValue);
            dynamic original = originalCustomer;
            //original.Name = strName;
            original.Name = nameValue;

            return Ok(originalCustomer);
        }
    }
}