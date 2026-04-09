using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, create a temporary integer column
            migrationBuilder.AddColumn<int>(
                name: "Type_New",
                table: "trips",
                type: "integer",
                nullable: true);

            // Convert existing string data to integer values
            migrationBuilder.Sql(
                @"UPDATE trips SET ""Type_New"" = CASE 
                    WHEN ""Type"" = 'Workshop' THEN 1
                    ELSE 0 
                END");

            // Drop the old string column
            migrationBuilder.DropColumn(
                table: "trips",
                name: "Type");

            // Rename the new column to replace the old one
            migrationBuilder.RenameColumn(
                table: "trips",
                name: "Type_New",
                newName: "Type");

            // Make the column not nullable
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "trips",
                type: "integer",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "trips",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
