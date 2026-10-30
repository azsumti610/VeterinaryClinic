using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VeterinaryClinic.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(120)]
        public string? FullName { get; set; }
        public ICollection<Examination> Examinations { get; set; }
    = new List<Examination>();
    }
}
