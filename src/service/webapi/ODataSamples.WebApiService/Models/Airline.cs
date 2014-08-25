namespace ODataSamples.WebApiService.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Airline
    {
        [Key]
        public string AirlineCode { get; set; }

        [Required]
        public string Name { get; set; }
    }
}