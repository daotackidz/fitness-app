using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);
        builder.Property(x => x.CoverImageId).HasColumnName("cover_image_id");
        builder.Property(x => x.Author).HasColumnName("author").HasMaxLength(100);
        builder.Property(x => x.PublishedAt).HasColumnName("published_at");

        builder.HasIndex(x => x.Category).HasDatabaseName("idx_articles_category");
        builder.HasIndex(x => x.PublishedAt).HasDatabaseName("idx_articles_published_at");

        builder.HasOne(x => x.CoverImage).WithMany().HasForeignKey(x => x.CoverImageId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("videos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.VideoFileId).HasColumnName("video_file_id").IsRequired();
        builder.Property(x => x.ThumbnailImageId).HasColumnName("thumbnail_image_id");
        builder.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);

        builder.HasIndex(x => x.Category).HasDatabaseName("idx_videos_category");

        builder.HasOne(x => x.VideoFile).WithMany().HasForeignKey(x => x.VideoFileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ThumbnailImage).WithMany().HasForeignKey(x => x.ThumbnailImageId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.FavoritableType).HasColumnName("favoritable_type").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.FavoritableId).HasColumnName("favoritable_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.UserId, x.FavoritableType, x.FavoritableId }).IsUnique().HasDatabaseName("idx_favorites_unique");
        builder.HasIndex(x => x.UserId).HasDatabaseName("idx_favorites_user_id");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}
