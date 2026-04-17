using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHighlightsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_organizers_OrganizerId",
                table: "trips");

            migrationBuilder.CreateTable(
                name: "highlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TripId = table.Column<int>(type: "integer", nullable: false),
                    Item = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_highlights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_highlights_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_highlights_TripId",
                table: "highlights",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_organizers_OrganizerId",
                table: "trips",
                column: "OrganizerId",
                principalTable: "organizers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_organizers_OrganizerId",
                table: "trips");

            migrationBuilder.DropTable(
                name: "highlights");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_organizers_OrganizerId",
                table: "trips",
                column: "OrganizerId",
                principalTable: "organizers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
