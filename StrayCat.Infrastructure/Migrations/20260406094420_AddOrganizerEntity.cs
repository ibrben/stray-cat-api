using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organizers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AvatarInitial = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    InviteTokenUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizers", x => x.Id);
                });

            // Clear OrganizerId column in trips table before adding foreign key
            migrationBuilder.Sql(
                @"ALTER TABLE trips ALTER COLUMN ""OrganizerId"" DROP NOT NULL");

            migrationBuilder.Sql(
                @"UPDATE trips SET ""OrganizerId"" = NULL WHERE ""OrganizerId"" IS NOT NULL OR ""OrganizerId"" = ''");

            migrationBuilder.CreateIndex(
                name: "IX_trips_OrganizerId",
                table: "trips",
                column: "OrganizerId");

            // Note: Foreign key constraint removed for now to allow NULL values
            // Will be added later when we have actual organizer data
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organizers");

            migrationBuilder.DropIndex(
                name: "IX_trips_OrganizerId",
                table: "trips");
        }
    }
}
