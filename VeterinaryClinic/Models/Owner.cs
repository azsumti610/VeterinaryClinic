using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class Owner
    {
        public int OwnerId { get; set; }
        [Required]
        [StringLength(80)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(80)]
        public string LastName { get; set; } = string.Empty;
        [Phone]
        [StringLength(32)]
        public string? PhoneNumber { get; set; }
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
