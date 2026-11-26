namespace VeterinaryClinic.Domain.Entities
{
    public class Animal
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Species { get; private set; } = string.Empty;
        public string? Breed { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public AnimalSex Sex { get; private set; }
        public string? DistinguishingMarks { get; private set; }
        public string? HealthNotes { get; private set; }
        public AnimalStatus Status { get; private set; }
        private Animal()
        {
        }
        public Animal(
            string name,
            string species,
            string? breed = null,
            DateTime? dateOfBirth = null,
            AnimalSex sex = AnimalSex.Unknown,
            string? distinguishingMarks = null,
            string? healthNotes = null)
        {
            UpdateDetails(
                name,
                species,
                breed,
                dateOfBirth,
                sex,
                distinguishingMarks,
                healthNotes);
            Id = Guid.NewGuid();
            Status = AnimalStatus.Active;
        }
        public void UpdateDetails(
            string name,
            string species,
            string? breed,
            DateTime? dateOfBirth,
            AnimalSex sex,
            string? distinguishingMarks,
            string? healthNotes)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "Името е задължително и може да е до 100 символа.");
            }
            if (string.IsNullOrWhiteSpace(species) ||
                species.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Видът е задължителен и може да е до 50 символа.");
            }
            if ((breed?.Length ?? 0) > 100)
            {
                throw new ArgumentException(
                    "Породата може да е до 100 символа.");
            }
            if (dateOfBirth.HasValue &&
                dateOfBirth.Value.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "Датата на раждане не може да бъде в бъдещето.");
            }
            if (!Enum.IsDefined(typeof(AnimalSex), sex))
            {
                throw new ArgumentException(
                    "Невалидна стойност за пола на животното.");
            }
            if ((distinguishingMarks?.Length ?? 0) > 1000)
            {
                throw new ArgumentException(
                    "Отличителните белези могат да са до 1000 символа.");
            }
            if ((healthNotes?.Length ?? 0) > 2000)
            {
                throw new ArgumentException(
                    "Здравните бележки могат да са до 2000 символа.");
            }
            Name = name.Trim();
            Species = species.Trim();
            Breed = string.IsNullOrWhiteSpace(breed)
                ? null
                : breed.Trim();
            DateOfBirth = dateOfBirth?.Date;
            Sex = sex;
            DistinguishingMarks =
                string.IsNullOrWhiteSpace(distinguishingMarks)
                    ? null
                    : distinguishingMarks.Trim();
            HealthNotes = string.IsNullOrWhiteSpace(healthNotes)
                ? null
                : healthNotes.Trim();
        }
    }
    
}
