using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}
