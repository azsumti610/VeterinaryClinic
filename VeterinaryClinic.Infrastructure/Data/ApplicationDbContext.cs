using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VeterinaryClinic.Infrastructure.Data
{
    public class ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

    }
}
