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
        builder.Property(x => x.CoverImage).HasColumnName("cover_image").HasMaxLength(255);
        builder.Property(x => x.Author).HasColumnName("author").HasMaxLength(100);
        builder.Property(x => x.PublishedAt).HasColumnName("published_at");

        builder.HasIndex(x => x.Category).HasDatabaseName("idx_articles_category");
        builder.HasIndex(x => x.PublishedAt).HasDatabaseName("idx_articles_published_at");
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
        builder.Property(x => x.VideoUrl).HasColumnName("video_url").HasMaxLength(255);
        builder.Property(x => x.ThumbnailUrl).HasColumnName("thumbnail_url").HasMaxLength(255);
        builder.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);

        builder.HasIndex(x => x.Category).HasDatabaseName("idx_videos_category");
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
