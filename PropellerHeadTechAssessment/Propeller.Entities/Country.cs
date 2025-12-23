using System.ComponentModel.DataAnnotations;

namespace Propeller.Entities
{
    public class Country
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
