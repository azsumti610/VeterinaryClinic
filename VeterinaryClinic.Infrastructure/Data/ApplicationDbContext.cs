using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VeterinaryClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace VeterinaryClinic.Infrastructure.Data
{
    public class ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Animal> Animals => Set<Animal>();
    }
}
