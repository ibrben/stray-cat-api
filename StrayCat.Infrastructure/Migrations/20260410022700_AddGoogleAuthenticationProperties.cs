using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAuthenticationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "organizers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "organizers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMobileSharing",
                table: "organizers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "organizers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "organizers");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "organizers");

            migrationBuilder.DropColumn(
                name: "AllowMobileSharing",
                table: "organizers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "organizers");
        }
    }
}
