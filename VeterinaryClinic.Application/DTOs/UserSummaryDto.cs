using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.DTOs
{
    public sealed class UserSummaryDto
    {
        public string Id { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Roles { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
