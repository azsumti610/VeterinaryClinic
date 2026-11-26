using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Application.Services.AnimalServices
{
    public sealed class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _repository;
        public AnimalService(IAnimalRepository repository)
        {
            _repository = repository;
        }
        public async Task<AnimalDto> AddAnimalAsync(
            CreateAnimalDto dto,
            string ownerId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
            var animal = new Animal(
                dto.Name,
                dto.Species,
                dto.Breed,
                dto.DateOfBirth,
                dto.Sex,
                dto.DistinguishingMarks,
                dto.HealthNotes);
            var ownership = new AnimalOwnership(
                animal.Id,
                ownerId,
                isPrimary: true,
                confirmedByUserId: ownerId);
            await _repository.AddWithOwnerAsync(
                animal,
                ownership,
                cancellationToken);
            return MapToDto(animal);
        }
        public async Task<IReadOnlyList<AnimalDto>> GetMyAnimalsAsync(
            string ownerId,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            var animals = await _repository.GetOwnedAsync(
                ownerId,
                search,
                cancellationToken);
            return animals.Select(MapToDto).ToList();
        }
        public async Task<AnimalDto?> FindAnimalAsync(
            Guid animalId,
            string ownerId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            if (animalId == Guid.Empty)
            {
                return null;
            }
            var animal = await _repository.GetOwnedByIdAsync(
                animalId,
                ownerId,
                cancellationToken);
            return animal is null ? null : MapToDto(animal);
        }
        public async Task<bool> EditAnimalAsync(
    Guid animalId,
    EditAnimalDto dto,
    string ownerId,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);

            if (animalId == Guid.Empty)
            {
                return false;
            }

            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);

            return await _repository.UpdateOwnedAsync(
                animalId,
                ownerId,
                dto.Name,
                dto.Species,
                dto.Breed,
                dto.DateOfBirth,
                dto.Sex,
                dto.DistinguishingMarks,
                dto.HealthNotes,
                cancellationToken);
        }
        private static AnimalDto MapToDto(Animal animal)
        {
            return new AnimalDto
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                Breed = animal.Breed,
                DateOfBirth = animal.DateOfBirth,
                Sex = animal.Sex,
                DistinguishingMarks = animal.DistinguishingMarks,
                HealthNotes = animal.HealthNotes,
                Status = animal.Status.ToString()
            };
        }
    }
}
