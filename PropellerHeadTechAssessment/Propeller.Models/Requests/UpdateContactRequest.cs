using System.ComponentModel.DataAnnotations;

namespace Propeller.Models.Requests
{
    public class UpdateContactRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string? CustomerID { get; set; }
    }
}
