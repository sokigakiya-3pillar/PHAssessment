using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Propeller.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Role { get; set; }

        [Required, MaxLength(5), MinLength(5)]
        public string Locale { get; set; } = string.Empty;

        [Required, MaxLength(3), MinLength(3)]
        public string CountryCode { get; set; } = string.Empty;

    }
}
