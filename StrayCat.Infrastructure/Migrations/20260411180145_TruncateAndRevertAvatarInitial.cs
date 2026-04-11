using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TruncateAndRevertAvatarInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Truncate existing AvatarInitial values to 10 characters before reducing column length
            migrationBuilder.Sql(
                "UPDATE organizers SET \"AvatarInitial\" = SUBSTRING(\"AvatarInitial\", 1, 10) WHERE LENGTH(\"AvatarInitial\") > 10");
            
            migrationBuilder.AlterColumn<string>(
                name: "AvatarInitial",
                table: "organizers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AvatarInitial",
                table: "organizers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }
    }
}
