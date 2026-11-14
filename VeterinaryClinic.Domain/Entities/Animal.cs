namespace VeterinaryClinic.Domain.Entities
{
    public class Animal
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Species { get; private set; } = string.Empty;

        public string? Breed { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public AnimalStatus Status { get; private set; }

        private Animal()
        {
        }

        public Animal(
            string name,
            string species,
            string? breed = null,
            DateTime? dateOfBirth = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Името на животното е задължително.");

            if (string.IsNullOrWhiteSpace(species))
                throw new ArgumentException("Видът на животното е задължителен.");

            if (dateOfBirth.HasValue && dateOfBirth.Value.Date > DateTime.Today)
                throw new ArgumentException("Датата на раждане не може да бъде в бъдещето.");

            Id = Guid.NewGuid();
            Name = name.Trim();
            Species = species.Trim();
            Breed = string.IsNullOrWhiteSpace(breed) ? null : breed.Trim();
            DateOfBirth = dateOfBirth;
            Status = AnimalStatus.Active;

        }
    }
}
