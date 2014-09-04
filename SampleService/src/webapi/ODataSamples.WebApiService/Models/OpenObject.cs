namespace ODataSamples.WebApiService.Models
{
    using System.Collections.Generic;

    public abstract class OpenObject
    {
        //Open properties
        public Dictionary<string, object> OpenProperties { get; set; }

        public OpenObject()
        {
            OpenProperties = new Dictionary<string, object>();
        }
    }
}