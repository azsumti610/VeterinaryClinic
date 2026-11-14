using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Domain.Entities;

namespace VeterinaryClinic.Application.Services.AnimalServices
{
    public sealed class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _repository;

        public AnimalService(IAnimalRepository repository)
        {
            _repository = repository;
        }

        public async Task<AnimalDto> AddAnimalAsync(CreateAnimalDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var animal = new Animal(
                dto.Name,
                dto.Species,
                dto.Breed,
                dto.DateOfBirth);

            await _repository.AddAsync(animal);

            return MapToDto(animal);
        }

        public async Task<AnimalDto?> FindAnimalAsync(Guid id)
        {
            var animal = await _repository.GetByIdAsync(id);

            return animal is null ? null : MapToDto(animal);
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
                Status = animal.Status.ToString()
            };
        }
    }
}
