namespace FitBodyApp.Application.Admin;

public record AdminUserDto(Guid Id, string FullName, string Email, string? Phone, string Role, string Status, DateTime CreatedAt);

public record UpdateUserStatusRequest(string Status);
public record UpdateUserRoleRequest(string Role);

public record UpsertExerciseRequest(string Name, string? Description, string? MuscleGroup, string? Equipment,
    string DifficultyLevel, Guid? VideoFileId, Guid? ImageFileId, int? CaloriesEstimate, int? DurationMinutes);

public record UpsertRoutineRequest(string Name, string Level, string? Description, int? DurationWeeks,
    Guid? ImageFileId, int? DurationMinutes, int? CaloriesEstimate, bool IsFeatured);

public record UpsertMealPlanRequest(string Name, string? Goal, string? Description, int? TotalCalories);

public record UpsertArticleRequest(string Title, string? Content, string? Category, Guid? CoverImageId, string? Author);

public record UpsertVideoRequest(string Title, string? Description, Guid VideoFileId, Guid? ThumbnailImageId, int? DurationSeconds, string? Category, int? CaloriesEstimate, int? ExerciseCount);

public record UpsertFaqRequest(string Question, string Answer, string? Category);

public record UpsertChallengeRequest(string Name, string? Description, string Type, DateOnly StartDate, DateOnly EndDate,
    string? GoalMetric, string? Reward, Guid? ImageFileId);

public record ReportedContentDto(Guid Id, Guid ReporterUserId, string ReportableType, Guid ReportableId, string? Reason, string Status, DateTime CreatedAt);

public record UpdateReportedContentStatusRequest(string Status);

public record AdminSupportTicketDto(Guid Id, Guid UserId, string UserFullName, string Subject, string Status, DateTime CreatedAt);

public record UpdateTicketStatusRequest(string Status);

public record DashboardSummaryDto(int TotalUsers, int NewUsersToday, int NewUsersThisWeek, int OpenTickets,
    int PendingReports, List<TopContentDto> TopRoutines, List<TopContentDto> TopArticles);

public record TopContentDto(Guid Id, string Name, int Score);

public record TimeSeriesPointDto(DateOnly Date, int Count);

public record ContentDistributionDto(string Label, int Count);

public record CategoryDto(Guid Id, string Name, string Type, int UsageCount, DateTime CreatedAt);

public record UpsertCategoryRequest(string Name, string Type);

public record CreateStaffUserRequest(string FullName, string Email, string? Phone, string Password, string Role);
