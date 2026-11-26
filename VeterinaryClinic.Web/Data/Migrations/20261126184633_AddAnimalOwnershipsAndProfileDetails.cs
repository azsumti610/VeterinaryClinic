using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinaryClinic.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimalOwnershipsAndProfileDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sex",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DistinguishingMarks",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthNotes",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnimalOwnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalOwnerships", x => x.Id);
                    table.CheckConstraint("CK_AnimalOwnerships_PrimaryMustBeActive", "[IsPrimary] = 0 OR [IsActive] = 1");
                    table.ForeignKey(
                        name: "FK_AnimalOwnerships_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimalOwnerships_AspNetUsers_ConfirmedByUserId",
                        column: x => x.ConfirmedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimalOwnerships_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalOwnerships_ActivePrimaryOwner",
                table: "AnimalOwnerships",
                column: "AnimalId",
                unique: true,
                filter: "[IsPrimary] = 1 AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalOwnerships_AnimalId_OwnerId",
                table: "AnimalOwnerships",
                columns: new[] { "AnimalId", "OwnerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalOwnerships_ConfirmedByUserId",
                table: "AnimalOwnerships",
                column: "ConfirmedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalOwnerships_OwnerId",
                table: "AnimalOwnerships",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalOwnerships");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "DistinguishingMarks",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "HealthNotes",
                table: "Animals");
        }
    }
}
