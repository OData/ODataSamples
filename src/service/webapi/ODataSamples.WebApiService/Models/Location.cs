namespace ODataSamples.WebApiService.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Spatial;

    public class Location : OpenObject
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public City City { get; set; }
    }

    public class AirportLocation : Location
    {
        [Required]
        public GeographyPoint Loc { get; set; }
    }

    public class EventLocation : Location
    {
        public string BuildingInfo { get; set; }
    }
}
