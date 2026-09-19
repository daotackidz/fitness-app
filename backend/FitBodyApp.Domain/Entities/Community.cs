using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class ForumPost
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class PostLike
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ForumPost Post { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ForumPost Post { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Challenge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChallengeType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? GoalMetric { get; set; }
    public string? Reward { get; set; }

    public ICollection<ChallengeParticipant> Participants { get; set; } = new List<ChallengeParticipant>();
}

public class ChallengeParticipant
{
    public Guid Id { get; set; }
    public Guid ChallengeId { get; set; }
    public Guid UserId { get; set; }
    public decimal Progress { get; set; }
    public int? Rank { get; set; }
    public DateTime JoinedAt { get; set; }

    public Challenge Challenge { get; set; } = null!;
    public User User { get; set; } = null!;
}
