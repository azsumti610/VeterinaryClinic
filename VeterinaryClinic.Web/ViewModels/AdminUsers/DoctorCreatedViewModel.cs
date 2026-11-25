namespace VeterinaryClinic.Web.ViewModels.AdminUsers;

public sealed class DoctorCreatedViewModel
{
    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string ActivationUrl { get; init; } = string.Empty;
}