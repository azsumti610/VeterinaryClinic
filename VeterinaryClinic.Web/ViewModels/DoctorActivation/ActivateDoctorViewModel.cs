using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Web.ViewModels.DoctorActivation;

public sealed class ActivateDoctorViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведи парола.")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "Паролата трябва да е между 6 и 100 символа.")]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Потвърди паролата.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Паролите не съвпадат.")]
    [Display(Name = "Потвърди паролата")]
    public string ConfirmPassword { get; set; } = string.Empty;
}