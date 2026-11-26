using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeterinaryClinic.Domain.Entities;
using VeterinaryClinic.Infrastructure.Identity;

namespace VeterinaryClinic.Infrastructure.Data.Configurations;

public sealed class AnimalOwnershipConfiguration
    : IEntityTypeConfiguration<AnimalOwnership>
{
    public void Configure(
        EntityTypeBuilder<AnimalOwnership> builder)
    {
        builder.ToTable("AnimalOwnerships", table =>
        {
            table.HasCheckConstraint(
                "CK_AnimalOwnerships_PrimaryMustBeActive",
                "[IsPrimary] = 0 OR [IsActive] = 1");
        });

        builder.HasKey(ownership => ownership.Id);

        builder.Property(ownership => ownership.Id)
            .ValueGeneratedNever();

        builder.Property(ownership => ownership.OwnerId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(ownership => ownership.ConfirmedByUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(ownership => ownership.ConfirmedAtUtc)
            .IsRequired();

        builder.HasOne<Animal>()
            .WithMany()
            .HasForeignKey(ownership => ownership.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ownership => ownership.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ownership => ownership.ConfirmedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Един потребител има само една връзка с дадено животно.
        builder.HasIndex(ownership => new
        {
            ownership.AnimalId,
            ownership.OwnerId
        })
        .IsUnique();

        // Най-много един действащ основен собственик за животно.
        builder.HasIndex(ownership => ownership.AnimalId)
            .IsUnique()
            .HasFilter("[IsPrimary] = 1 AND [IsActive] = 1")
            .HasDatabaseName(
                "IX_AnimalOwnerships_ActivePrimaryOwner");
    }
}