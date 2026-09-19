namespace FitBodyApp.Application.Community;

public record ForumPostDto(Guid Id, Guid UserId, string UserFullName, string Title, string? Content, string? ImageUrl, int LikesCount, DateTime CreatedAt);

public record CreateForumPostRequest(string Title, string? Content, string? ImageUrl);

public record CommentDto(Guid Id, Guid UserId, string UserFullName, string Content, DateTime CreatedAt);

public record CreateCommentRequest(string Content);

public record ChallengeParticipantDto(Guid UserId, string UserFullName, decimal Progress, int? Rank, DateTime JoinedAt);

public record ChallengeDto(Guid Id, string Name, string? Description, string Type, DateOnly StartDate, DateOnly EndDate,
    string? GoalMetric, string? Reward, List<ChallengeParticipantDto>? Leaderboard);

public record UpdateParticipantProgressRequest(decimal Progress);
