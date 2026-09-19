using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(150);
        builder.Property(x => x.Message).HasColumnName("message");
        builder.Property(x => x.IsRead).HasColumnName("is_read").HasDefaultValue(false);
        builder.Property(x => x.ScheduledAt).HasColumnName("scheduled_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.UserId, x.IsRead }).HasDatabaseName("idx_notifications_user_read");
        builder.HasIndex(x => x.ScheduledAt).HasDatabaseName("idx_notifications_scheduled_at");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.ToTable("faqs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Question).HasColumnName("question").HasMaxLength(255);
        builder.Property(x => x.Answer).HasColumnName("answer");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);

        builder.HasIndex(x => x.Category).HasDatabaseName("idx_faqs_category");
    }
}

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Subject).HasColumnName("subject").HasMaxLength(200);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).HasDefaultValue(Domain.Enums.SupportTicketStatus.Open);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.UserId, x.Status }).HasDatabaseName("idx_support_tickets_user_status");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.ToTable("support_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TicketId).HasColumnName("ticket_id");
        builder.Property(x => x.SenderType).HasColumnName("sender_type").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Message).HasColumnName("message");
        builder.Property(x => x.SentAt).HasColumnName("sent_at");

        builder.HasIndex(x => x.TicketId).HasDatabaseName("idx_support_messages_ticket_id");

        builder.HasOne(x => x.Ticket).WithMany(x => x.Messages).HasForeignKey(x => x.TicketId);
    }
}
