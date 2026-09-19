using System.Linq.Expressions;
using FitBodyApp.Domain.Entities;

namespace FitBodyApp.Application.Community;

public record ForumPostDto(Guid Id, Guid UserId, string UserFullName, string Title, string? Content, string? ImageUrl,
    int LikesCount, DateTime CreatedAt, Guid? ImageFileId);

public record CreateForumPostRequest(string Title, string? Content, Guid? ImageFileId);

public record CommentDto(Guid Id, Guid UserId, string UserFullName, string Content, DateTime CreatedAt);

public record CreateCommentRequest(string Content);

public record ChallengeParticipantDto(Guid UserId, string UserFullName, decimal Progress, int? Rank, DateTime JoinedAt);

public record ChallengeDto(Guid Id, string Name, string? Description, string Type, DateOnly StartDate, DateOnly EndDate,
    string? GoalMetric, string? Reward, List<ChallengeParticipantDto>? Leaderboard);

public record UpdateParticipantProgressRequest(decimal Progress);

public static class ForumPostMappings
{
    public static readonly Expression<Func<ForumPost, ForumPostDto>> ToDto = p => new ForumPostDto(
        p.Id, p.UserId, p.User.FullName, p.Title, p.Content,
        p.ImageFile != null ? p.ImageFile.Url : null,
        p.LikesCount, p.CreatedAt, p.ImageFileId);
}
