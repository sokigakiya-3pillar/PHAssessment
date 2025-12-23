using System.ComponentModel.DataAnnotations;

namespace Propeller.Entities
{
    public class CustomerStatus
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        public ICollection<Customer> Customers { get; set; } 
    }
}
