namespace FitBodyApp.Application.Content;

public record ArticleDto(Guid Id, string Title, string? Content, string? Category, string? CoverImage, string? Author, DateTime? PublishedAt);

public record VideoDto(Guid Id, string Title, string? Description, string VideoUrl, string? ThumbnailUrl, int? DurationSeconds, string? Category);

public record CreateFavoriteRequest(string FavoritableType, Guid FavoritableId);

public record FavoriteDto(Guid Id, string FavoritableType, Guid FavoritableId, DateTime CreatedAt);
