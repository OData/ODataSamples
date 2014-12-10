using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ODataSamples.CustomFormatService
{
    public class BusinessCard
    {
        public BusinessCard()
        {
            this.DynProperties = new Dictionary<string, object>();
        }

        public string N { get; set; }

        public string FN { get; set; }

        public string ORG { get; set; }

        public IDictionary<string, object> DynProperties { get; set; }
    }
}