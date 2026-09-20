using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitBodyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeScreenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                table: "exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "challenges",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_challenges_image_file_id",
                table: "challenges",
                column: "image_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_challenges_image_files_image_file_id",
                table: "challenges",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_challenges_image_files_image_file_id",
                table: "challenges");

            migrationBuilder.DropIndex(
                name: "IX_challenges_image_file_id",
                table: "challenges");

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "challenges");
        }
    }
}
