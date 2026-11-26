using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Domain.Entities;

namespace VeterinaryClinic.Application.DTOs;

public sealed class CreateAnimalDto
{
    [Required(ErrorMessage = "Името е задължително.")]
    [StringLength(100, ErrorMessage = "Името може да е до 100 символа.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Видът е задължителен.")]
    [StringLength(50, ErrorMessage = "Видът може да е до 50 символа.")]
    public string Species { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Породата може да е до 100 символа.")]
    public string? Breed { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [EnumDataType(
        typeof(AnimalSex),
        ErrorMessage = "Избери валидна стойност за пола.")]
    public AnimalSex Sex { get; set; } = AnimalSex.Unknown;

    [StringLength(
        1000,
        ErrorMessage = "Отличителните белези могат да са до 1000 символа.")]
    public string? DistinguishingMarks { get; set; }

    [StringLength(
        2000,
        ErrorMessage = "Здравните бележки могат да са до 2000 символа.")]
    public string? HealthNotes { get; set; }
}