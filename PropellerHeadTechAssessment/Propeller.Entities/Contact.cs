using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Propeller.Entities
{
    public class Contact
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]  
        public string LastName { get; set; } = string.Empty;


        public string EMail { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;


        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
