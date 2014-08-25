namespace ODataSamples.WebApiService.Models
{
    using System.ComponentModel.DataAnnotations;

    public class City
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
