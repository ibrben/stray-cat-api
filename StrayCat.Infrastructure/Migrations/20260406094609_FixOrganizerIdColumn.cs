using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrganizerIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First create a temporary integer column
            migrationBuilder.AddColumn<int>(
                table: "trips",
                name: "OrganizerId_New",
                type: "integer",
                nullable: true);

            // Update existing records to use default value 0 for non-null OrganizerId
            migrationBuilder.Sql(
                @"UPDATE trips SET ""OrganizerId_New"" = 0 WHERE ""OrganizerId"" IS NULL OR ""OrganizerId"" = ''");

            // Drop the old string column
            migrationBuilder.DropColumn(
                table: "trips",
                name: "OrganizerId");

            // Rename the new column to replace the old one
            migrationBuilder.RenameColumn(
                table: "trips",
                name: "OrganizerId_New",
                newName: "OrganizerId");

            // Make the column not nullable
            migrationBuilder.AlterColumn<int>(
                table: "trips",
                name: "OrganizerId",
                type: "integer",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                table: "trips",
                name: "OrganizerId",
                type: "character varying(50)",
                nullable: false);
        }
    }
}
