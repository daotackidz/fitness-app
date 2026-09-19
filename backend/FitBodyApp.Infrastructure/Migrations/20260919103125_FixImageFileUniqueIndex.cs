using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitBodyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixImageFileUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_image_files_uploaded_by",
                table: "image_files");

            migrationBuilder.CreateIndex(
                name: "idx_image_files_uploaded_by",
                table: "image_files",
                column: "uploaded_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_image_files_uploaded_by",
                table: "image_files");

            migrationBuilder.CreateIndex(
                name: "idx_image_files_uploaded_by",
                table: "image_files",
                column: "uploaded_by_user_id",
                unique: true);
        }
    }
}
