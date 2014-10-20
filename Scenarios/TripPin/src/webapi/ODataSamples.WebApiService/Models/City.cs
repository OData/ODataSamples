namespace ODataSamples.WebApiService.Models
{
    using System.ComponentModel.DataAnnotations;

    public class City
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string CountryRegion { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
