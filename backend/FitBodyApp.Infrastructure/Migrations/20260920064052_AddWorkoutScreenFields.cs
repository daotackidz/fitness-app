using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitBodyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutScreenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "calories_estimate",
                table: "routines",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                table: "routines",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "routines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_featured",
                table: "routines",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "duration_seconds",
                table: "routine_exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "round_number",
                table: "routine_exercises",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_routines_image_file_id",
                table: "routines",
                column: "image_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_routines_image_files_image_file_id",
                table: "routines",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_routines_image_files_image_file_id",
                table: "routines");

            migrationBuilder.DropIndex(
                name: "IX_routines_image_file_id",
                table: "routines");

            migrationBuilder.DropColumn(
                name: "calories_estimate",
                table: "routines");

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                table: "routines");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "routines");

            migrationBuilder.DropColumn(
                name: "is_featured",
                table: "routines");

            migrationBuilder.DropColumn(
                name: "duration_seconds",
                table: "routine_exercises");

            migrationBuilder.DropColumn(
                name: "round_number",
                table: "routine_exercises");
        }
    }
}
