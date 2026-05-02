using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "trips");
        }
    }
}
