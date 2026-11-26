using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Domain.Entities;
using VeterinaryClinic.Infrastructure.Data.Configurations;
using VeterinaryClinic.Infrastructure.Identity;

namespace VeterinaryClinic.Infrastructure.Data
{
    public class ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<AnimalOwnership> AnimalOwnerships
                => Set<AnimalOwnership>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(
                new AnimalOwnershipConfiguration());
        }
    }

}
