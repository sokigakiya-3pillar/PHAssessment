using System.ComponentModel.DataAnnotations;

namespace Propeller.Models.Requests
{
    public class UpdateCustomerRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Status { get; set; }
    }
}
