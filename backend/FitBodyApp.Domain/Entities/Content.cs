using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? CoverImage { get; set; }
    public string? Author { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class Video
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Category { get; set; }
}

public class Favorite
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public FavoritableType FavoritableType { get; set; }
    public Guid FavoritableId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
