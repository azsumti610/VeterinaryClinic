using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;
using VeterinaryClinic.Domain.Entities;

namespace VeterinaryClinic.Application.Interfaces
{
    public interface IAnimalRepository
    {
        Task AddWithOwnerAsync(
        Animal animal,
        AnimalOwnership ownership,
        CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Animal>> GetOwnedAsync(
            string ownerId,
            string? search = null,
            CancellationToken cancellationToken = default);
        Task<Animal?> GetOwnedByIdAsync(
            Guid animalId,
            string ownerId,
            CancellationToken cancellationToken = default);
    }
}
