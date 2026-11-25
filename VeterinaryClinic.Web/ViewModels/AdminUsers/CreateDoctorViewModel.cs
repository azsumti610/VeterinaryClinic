using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Web.ViewModels.AdminUsers;

public sealed class CreateDoctorViewModel
{
    [Required(ErrorMessage = "Въведи потребителско име.")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "Името трябва да е между 3 и 50 символа.")]
    [RegularExpression(
        @"[a-zA-Z0-9._-]+",
        ErrorMessage =
            "Използвай латински букви, цифри, точка, тире или долна черта.")]
    [Display(Name = "Потребителско име")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведи имейл.")]
    [EmailAddress(ErrorMessage = "Въведи валиден имейл.")]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;
}