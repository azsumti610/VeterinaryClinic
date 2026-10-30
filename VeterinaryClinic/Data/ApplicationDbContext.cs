using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;

namespace VeterinaryClinic.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AnimalType> AnimalTypes { get; set; }
            = null!;

        public DbSet<Breed> Breeds { get; set; }
            = null!;

        public DbSet<Owner> Owners { get; set; }
            = null!;

        public DbSet<Animal> Animals { get; set; }
            = null!;

        public DbSet<Examination> Examinations { get; set; }
            = null!;
    }
}
