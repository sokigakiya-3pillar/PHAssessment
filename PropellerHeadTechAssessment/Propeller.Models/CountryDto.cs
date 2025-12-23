using System.ComponentModel.DataAnnotations;

namespace Propeller.Models
{
    public class CountryDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        public string DefaultLocale { get; set; } = string.Empty;

    }
}
