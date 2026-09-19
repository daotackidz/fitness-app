using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> builder)
    {
        builder.ToTable("admin_audit_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AdminUserId).HasColumnName("admin_user_id");
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(100);
        builder.Property(x => x.TargetType).HasColumnName("target_type").HasMaxLength(50);
        builder.Property(x => x.TargetId).HasColumnName("target_id");
        builder.Property(x => x.MetadataJson).HasColumnName("metadata").HasColumnType("jsonb");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.AdminUserId).HasDatabaseName("idx_admin_audit_logs_admin_user_id");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_admin_audit_logs_created_at");

        builder.HasOne(x => x.AdminUser).WithMany().HasForeignKey(x => x.AdminUserId);
    }
}

public class ReportedContentConfiguration : IEntityTypeConfiguration<ReportedContent>
{
    public void Configure(EntityTypeBuilder<ReportedContent> builder)
    {
        builder.ToTable("reported_contents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReporterUserId).HasColumnName("reporter_user_id");
        builder.Property(x => x.ReportableType).HasColumnName("reportable_type").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ReportableId).HasColumnName("reportable_id");
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).HasDefaultValue(Domain.Enums.ReportStatus.Pending);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.Status).HasDatabaseName("idx_reported_contents_status");

        builder.HasOne(x => x.ReporterUser).WithMany().HasForeignKey(x => x.ReporterUserId);
    }
}
