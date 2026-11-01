using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Models
{
    public class OwnerProfileViewModel
    {
        [Required(ErrorMessage = "Въведете име.")]
        [StringLength(80)]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Въведете фамилия.")]
        [StringLength(80)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Въведете валиден телефонен номер.")]
        [StringLength(32)]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "Въведете валиден имейл адрес.")]
        [StringLength(254)]
        [Display(Name = "Имейл за контакт")]
        public string? Email { get; set; }
        [StringLength(300)]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }
    }
}
