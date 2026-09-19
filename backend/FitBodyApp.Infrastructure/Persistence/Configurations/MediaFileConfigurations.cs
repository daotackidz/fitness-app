using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class VideoFileConfiguration : IEntityTypeConfiguration<VideoFile>
{
    public void Configure(EntityTypeBuilder<VideoFile> builder)
    {
        builder.ToTable("video_files");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasColumnName("url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.BlobName).HasColumnName("blob_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Container).HasColumnName("container").HasMaxLength(100).IsRequired();
        builder.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(100);
        builder.Property(x => x.SizeBytes).HasColumnName("size_bytes");
        builder.Property(x => x.UploadedByUserId).HasColumnName("uploaded_by_user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.UploadedByUserId).HasDatabaseName("idx_video_files_uploaded_by").IsUnique(false);
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_video_files_created_at");

        builder.HasOne(x => x.UploadedByUser).WithMany().HasForeignKey(x => x.UploadedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ImageFileConfiguration : IEntityTypeConfiguration<ImageFile>
{
    public void Configure(EntityTypeBuilder<ImageFile> builder)
    {
        builder.ToTable("image_files");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasColumnName("url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.BlobName).HasColumnName("blob_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Container).HasColumnName("container").HasMaxLength(100).IsRequired();
        builder.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(100);
        builder.Property(x => x.SizeBytes).HasColumnName("size_bytes");
        builder.Property(x => x.UploadedByUserId).HasColumnName("uploaded_by_user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.UploadedByUserId).HasDatabaseName("idx_image_files_uploaded_by").IsUnique(false);
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_image_files_created_at");

        builder.HasOne(x => x.UploadedByUser).WithMany().HasForeignKey(x => x.UploadedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
