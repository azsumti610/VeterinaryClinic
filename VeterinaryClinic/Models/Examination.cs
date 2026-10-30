using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class Examination
    {
        public int ExaminationId { get; set; }
        [Required]
        public int AnimalId { get; set; }
        public Animal Animal { get; set; } = null!;
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser Doctor { get; set; } = null!;
        [Required]
        public DateOnly ExamDate { get; set; }
        [Required]
        public TimeOnly ExamTime { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        [StringLength(1000)]
        public string? Diagnosis { get; set; }
        [StringLength(2000)]
        public string? Recommendations { get; set; }
    }
}
