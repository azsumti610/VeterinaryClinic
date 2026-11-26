using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;

namespace VeterinaryClinic.Application.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDto> AddAnimalAsync(
        CreateAnimalDto dto,
        string ownerId,
        CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AnimalDto>> GetMyAnimalsAsync(
            string ownerId,
            string? search = null,
            CancellationToken cancellationToken = default);
        Task<AnimalDto?> FindAnimalAsync(
            Guid animalId,
            string ownerId,
            CancellationToken cancellationToken = default);
        Task<bool> EditAnimalAsync(
    Guid animalId,
    EditAnimalDto dto,
    string ownerId,
    CancellationToken cancellationToken = default);
    }
}
