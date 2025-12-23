using System.ComponentModel.DataAnnotations;

namespace Propeller.Models.Requests
{
    public class CreateCustomerRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public ContactDto? Contact { get; set; }

    }
}
