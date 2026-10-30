using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class Breed
    {
        public int BreedId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        [Required]
        public int AnimalTypeId { get; set; }
        public AnimalType? AnimalType { get; set; }
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
