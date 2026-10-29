using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int BreedId { get; set; }
        public Breed Breed { get; set; } = null!;
        [Required]
        public int OwnerId { get; set; }
        public Owner Owner { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(500)]
        public string? ImagePath { get; set; }
        public ICollection<Examination> Examinations { get; set; }
            = new List<Examination>();
    }
}
