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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AnimalType>()
                .HasIndex(animalType => animalType.Name)
                .IsUnique();
            builder.Entity<Breed>()
                .HasIndex(breed => new { breed.AnimalTypeId, breed.Name })
                .IsUnique();
            builder.Entity<Breed>()
                .HasOne(breed => breed.AnimalType)
                .WithMany(animalType => animalType.Breeds)
                .HasForeignKey(breed => breed.AnimalTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Animal>()
                .HasOne(animal => animal.Breed)
                .WithMany(breed => breed.Animals)
                .HasForeignKey(animal => animal.BreedId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Animal>()
                .HasOne(animal => animal.Owner)
                .WithMany(owner => owner.Animals)
                .HasForeignKey(animal => animal.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Examination>()
                .Property(examination => examination.ExamDate)
                .HasColumnType("date");
            builder.Entity<Examination>()
                .Property(examination => examination.ExamTime)
                .HasColumnType("time");
            builder.Entity<Examination>()
                .HasOne(examination => examination.Animal)
                .WithMany(animal => animal.Examinations)
                .HasForeignKey(examination => examination.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Examination>()
                .HasOne(examination => examination.Doctor)
                .WithMany(user => user.Examinations)
                .HasForeignKey(examination => examination.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
