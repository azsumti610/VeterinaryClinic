using VeterinaryClinic.Application.DTOs;

namespace VeterinaryClinic.Web.ViewModels.Animals;

public sealed class AnimalListViewModel
{
    public string? Search { get; init; }

    public IReadOnlyList<AnimalDto> Animals { get; init; }
        = Array.Empty<AnimalDto>();
}