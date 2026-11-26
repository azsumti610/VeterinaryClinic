using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Domain.Entities
{
    public class AnimalOwnership
    {
        public Guid Id { get; private set; }
        public Guid AnimalId { get; private set; }
        public string OwnerId { get; private set; } = string.Empty;
        public bool IsPrimary { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime ConfirmedAtUtc { get; private set; }
        public string ConfirmedByUserId { get; private set; }
            = string.Empty;
        private AnimalOwnership()
        {
        }
        public AnimalOwnership(
            Guid animalId,
            string ownerId,
            bool isPrimary,
            string confirmedByUserId)
        {
            if (animalId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Липсва идентификатор на животното.");
            }
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException(
                    "Липсва идентификатор на собственика.");
            }
            if (string.IsNullOrWhiteSpace(confirmedByUserId))
            {
                throw new ArgumentException(
                    "Липсва потребителят, потвърдил собствеността.");
            }
            Id = Guid.NewGuid();
            AnimalId = animalId;
            OwnerId = ownerId;
            IsPrimary = isPrimary;
            IsActive = true;
            ConfirmedAtUtc = DateTime.UtcNow;
            ConfirmedByUserId = confirmedByUserId;
        }
    }
}
