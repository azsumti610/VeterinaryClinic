using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.DTOs
{
    public sealed class AnimalDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
