using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataSamples.WebApiService.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public double Price { get; set; }
    }
}