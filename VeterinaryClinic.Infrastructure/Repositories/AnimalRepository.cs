using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Domain.Entities;
using VeterinaryClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using VeterinaryClinic.Infrastructure.Identity;


namespace VeterinaryClinic.Infrastructure.Repositories
{
    public sealed class AnimalRepository : IAnimalRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AnimalRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task AddWithOwnerAsync(
            Animal animal,
            AnimalOwnership ownership,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(animal);
            ArgumentNullException.ThrowIfNull(ownership);
            if (ownership.AnimalId != animal.Id ||
                !ownership.IsActive ||
                !ownership.IsPrimary ||
                ownership.OwnerId != ownership.ConfirmedByUserId)
            {
                throw new InvalidOperationException(
                    "Невалидна първоначална собственост на животното.");
            }
            var owner = await _userManager.FindByIdAsync(
                ownership.OwnerId);
            if (owner is null ||
                !owner.IsActive ||
                !await _userManager.IsInRoleAsync(owner, "Owner"))
            {
                throw new InvalidOperationException(
                    "Само активен собственик може да добавя свое животно.");
            }
            _context.Animals.Add(animal);
            _context.AnimalOwnerships.Add(ownership);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<Animal>> GetOwnedAsync(
            string ownerId,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            var query = _context.Animals
                .AsNoTracking()
                .Where(animal =>
                    _context.AnimalOwnerships.Any(ownership =>
                        ownership.AnimalId == animal.Id &&
                        ownership.OwnerId == ownerId &&
                        ownership.IsActive));
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(animal =>
                    animal.Name.Contains(term) ||
                    animal.Species.Contains(term) ||
                    (animal.Breed != null &&
                     animal.Breed.Contains(term)));
            }
            return await query
                .OrderBy(animal => animal.Name)
                .ThenBy(animal => animal.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<Animal?> GetOwnedByIdAsync(
            Guid animalId,
            string ownerId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            return await _context.Animals
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    animal =>
                        animal.Id == animalId &&
                        _context.AnimalOwnerships.Any(ownership =>
                            ownership.AnimalId == animal.Id &&
                            ownership.OwnerId == ownerId &&
                            ownership.IsActive),
                    cancellationToken);
        }
    }
}
