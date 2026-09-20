using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitBodyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileScreenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "calories_estimate",
                table: "videos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exercise_count",
                table: "videos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "do_not_disturb_enabled",
                table: "user_settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lock_screen_enabled",
                table: "user_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "reminders_enabled",
                table: "user_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "sound_enabled",
                table: "user_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "vibrate_enabled",
                table: "user_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "calories_estimate",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "exercise_count",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "do_not_disturb_enabled",
                table: "user_settings");

            migrationBuilder.DropColumn(
                name: "lock_screen_enabled",
                table: "user_settings");

            migrationBuilder.DropColumn(
                name: "reminders_enabled",
                table: "user_settings");

            migrationBuilder.DropColumn(
                name: "sound_enabled",
                table: "user_settings");

            migrationBuilder.DropColumn(
                name: "vibrate_enabled",
                table: "user_settings");
        }
    }
}
