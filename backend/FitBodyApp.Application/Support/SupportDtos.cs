namespace FitBodyApp.Application.Support;

public record FaqDto(Guid Id, string Question, string Answer, string? Category);

public record NotificationDto(Guid Id, string Type, string Title, string? Message, bool IsRead, DateTime? ScheduledAt, DateTime CreatedAt);

public record UpdateNotificationRequest(bool IsRead);

public record CreateSupportTicketRequest(string Subject);

public record SupportTicketDto(Guid Id, string Subject, string Status, DateTime CreatedAt);

public record CreateSupportMessageRequest(string Message);

public record SupportMessageDto(Guid Id, Guid TicketId, string SenderType, string Message, DateTime SentAt);
