using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitBodyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFileTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnail_url",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "video_url",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "users");

            migrationBuilder.DropColumn(
                name: "photo_url",
                table: "progress_tracking");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "meals");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "forum_posts");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "food_items");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "video_url",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "cover_image",
                table: "articles");

            migrationBuilder.AddColumn<Guid>(
                name: "thumbnail_image_id",
                table: "videos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "video_file_id",
                table: "videos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "avatar_image_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "photo_image_id",
                table: "progress_tracking",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "meals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "forum_posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "food_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                table: "exercises",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "video_file_id",
                table: "exercises",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cover_image_id",
                table: "articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "image_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    container = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_image_files_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "video_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    container = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_video_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_video_files_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_videos_thumbnail_image_id",
                table: "videos",
                column: "thumbnail_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_videos_video_file_id",
                table: "videos",
                column: "video_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_avatar_image_id",
                table: "users",
                column: "avatar_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_progress_tracking_photo_image_id",
                table: "progress_tracking",
                column: "photo_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_meals_image_file_id",
                table: "meals",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_forum_posts_image_file_id",
                table: "forum_posts",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_food_items_image_file_id",
                table: "food_items",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercises_image_file_id",
                table: "exercises",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercises_video_file_id",
                table: "exercises",
                column: "video_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_cover_image_id",
                table: "articles",
                column: "cover_image_id");

            migrationBuilder.CreateIndex(
                name: "idx_image_files_created_at",
                table: "image_files",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_image_files_uploaded_by",
                table: "image_files",
                column: "uploaded_by_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_video_files_created_at",
                table: "video_files",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_video_files_uploaded_by",
                table: "video_files",
                column: "uploaded_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_image_files_cover_image_id",
                table: "articles",
                column: "cover_image_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_exercises_image_files_image_file_id",
                table: "exercises",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_exercises_video_files_video_file_id",
                table: "exercises",
                column: "video_file_id",
                principalTable: "video_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_food_items_image_files_image_file_id",
                table: "food_items",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_forum_posts_image_files_image_file_id",
                table: "forum_posts",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_meals_image_files_image_file_id",
                table: "meals",
                column: "image_file_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_progress_tracking_image_files_photo_image_id",
                table: "progress_tracking",
                column: "photo_image_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_image_files_avatar_image_id",
                table: "users",
                column: "avatar_image_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_videos_image_files_thumbnail_image_id",
                table: "videos",
                column: "thumbnail_image_id",
                principalTable: "image_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_videos_video_files_video_file_id",
                table: "videos",
                column: "video_file_id",
                principalTable: "video_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_image_files_cover_image_id",
                table: "articles");

            migrationBuilder.DropForeignKey(
                name: "FK_exercises_image_files_image_file_id",
                table: "exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_exercises_video_files_video_file_id",
                table: "exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_food_items_image_files_image_file_id",
                table: "food_items");

            migrationBuilder.DropForeignKey(
                name: "FK_forum_posts_image_files_image_file_id",
                table: "forum_posts");

            migrationBuilder.DropForeignKey(
                name: "FK_meals_image_files_image_file_id",
                table: "meals");

            migrationBuilder.DropForeignKey(
                name: "FK_progress_tracking_image_files_photo_image_id",
                table: "progress_tracking");

            migrationBuilder.DropForeignKey(
                name: "FK_users_image_files_avatar_image_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_videos_image_files_thumbnail_image_id",
                table: "videos");

            migrationBuilder.DropForeignKey(
                name: "FK_videos_video_files_video_file_id",
                table: "videos");

            migrationBuilder.DropTable(
                name: "image_files");

            migrationBuilder.DropTable(
                name: "video_files");

            migrationBuilder.DropIndex(
                name: "IX_videos_thumbnail_image_id",
                table: "videos");

            migrationBuilder.DropIndex(
                name: "IX_videos_video_file_id",
                table: "videos");

            migrationBuilder.DropIndex(
                name: "IX_users_avatar_image_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_progress_tracking_photo_image_id",
                table: "progress_tracking");

            migrationBuilder.DropIndex(
                name: "IX_meals_image_file_id",
                table: "meals");

            migrationBuilder.DropIndex(
                name: "IX_forum_posts_image_file_id",
                table: "forum_posts");

            migrationBuilder.DropIndex(
                name: "IX_food_items_image_file_id",
                table: "food_items");

            migrationBuilder.DropIndex(
                name: "IX_exercises_image_file_id",
                table: "exercises");

            migrationBuilder.DropIndex(
                name: "IX_exercises_video_file_id",
                table: "exercises");

            migrationBuilder.DropIndex(
                name: "IX_articles_cover_image_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "thumbnail_image_id",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "video_file_id",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "avatar_image_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "photo_image_id",
                table: "progress_tracking");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "meals");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "forum_posts");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "food_items");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "video_file_id",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "cover_image_id",
                table: "articles");

            migrationBuilder.AddColumn<string>(
                name: "thumbnail_url",
                table: "videos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_url",
                table: "videos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                table: "progress_tracking",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "meals",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "forum_posts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "food_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "exercises",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_url",
                table: "exercises",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_image",
                table: "articles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
