using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrayCat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogWritingInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "blogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Excerpt",
                table: "blogs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicationStatus",
                table: "blogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "blogs",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceInviteId",
                table: "blogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "blog_writing_invitations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OwnerUserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxSubmissions = table.Column<int>(type: "integer", nullable: false),
                    SubmissionCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TripId = table.Column<int>(type: "integer", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_writing_invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_blog_writing_invitations_organizers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "organizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_blog_writing_invitations_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blog_writing_invitations_OwnerUserId",
                table: "blog_writing_invitations",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_blog_writing_invitations_TokenHash",
                table: "blog_writing_invitations",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blog_writing_invitations_TripId",
                table: "blog_writing_invitations",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_writing_invitations");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "blogs");

            migrationBuilder.DropColumn(
                name: "Excerpt",
                table: "blogs");

            migrationBuilder.DropColumn(
                name: "PublicationStatus",
                table: "blogs");

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "blogs");

            migrationBuilder.DropColumn(
                name: "SourceInviteId",
                table: "blogs");
        }
    }
}
