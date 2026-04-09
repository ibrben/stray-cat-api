using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCaptionAndIsPrimaryFromTripImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "trip_images");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "trip_images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "trip_images",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "trip_images",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
