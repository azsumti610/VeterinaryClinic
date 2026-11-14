using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;

namespace VeterinaryClinic.Application.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDto> AddAnimalAsync(CreateAnimalDto dto);

        Task<AnimalDto?> FindAnimalAsync(Guid id);
    }
}
