using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.Users
{
    public sealed class CreateDoctorResult
    {
        public bool Succeeded { get; init; }
        public string? Error { get; init; }
        public string? UserId { get; init; }
        public string? PasswordSetupToken { get; init; }
    }
}
