using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Domain.Entities;
using VeterinaryClinic.Infrastructure.Identity;

namespace VeterinaryClinic.Infrastructure.Data
{
    public class ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Animal> Animals => Set<Animal>();
    }
}
