using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}

public class Faq
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class SupportTicket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
}

public class SupportMessage
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public SupportSenderType SenderType { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    public SupportTicket Ticket { get; set; } = null!;
}
