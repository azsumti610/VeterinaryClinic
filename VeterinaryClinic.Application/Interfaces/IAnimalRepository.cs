using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Domain.Entities;

namespace VeterinaryClinic.Application.Interfaces
{
    public interface IAnimalRepository
    {
        Task AddAsync(Animal animal);

        Task<Animal?> GetByIdAsync(Guid id);
    }
}
