using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class AdminAuditLog
{
    public Guid Id { get; set; }
    public Guid AdminUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public Guid? TargetId { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }

    public User AdminUser { get; set; } = null!;
}

public class ReportedContent
{
    public Guid Id { get; set; }
    public Guid ReporterUserId { get; set; }
    public ReportableType ReportableType { get; set; }
    public Guid ReportableId { get; set; }
    public string? Reason { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime CreatedAt { get; set; }

    public User ReporterUser { get; set; } = null!;
}
