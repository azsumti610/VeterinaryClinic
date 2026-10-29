using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class AnimalType
    {
        public int AnimalTypeId { get; set; }
        [Required]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Breed> Breeds { get; set; } = new List<Breed>();
    }
}
