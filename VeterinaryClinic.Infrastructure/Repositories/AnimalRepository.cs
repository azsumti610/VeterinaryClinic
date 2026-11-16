using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Domain.Entities;
using VeterinaryClinic.Infrastructure.Data;
namespace VeterinaryClinic.Infrastructure.Repositories
{
    public sealed class AnimalRepository : IAnimalRepository
    {
        private readonly ApplicationDbContext _context;

        public AnimalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Animal animal)
        {
            await _context.Set<Animal>().AddAsync(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<Animal?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Animal>()
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
