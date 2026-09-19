using System.Linq.Expressions;
using FitBodyApp.Domain.Entities;

namespace FitBodyApp.Application.Content;

public record ArticleDto(Guid Id, string Title, string? Content, string? Category, string? CoverImage, string? Author,
    DateTime? PublishedAt, Guid? CoverImageId);

public record VideoDto(Guid Id, string Title, string? Description, string VideoUrl, string? ThumbnailUrl,
    int? DurationSeconds, string? Category, Guid VideoFileId, Guid? ThumbnailImageId);

public record CreateFavoriteRequest(string FavoritableType, Guid FavoritableId);

public record FavoriteDto(Guid Id, string FavoritableType, Guid FavoritableId, DateTime CreatedAt);

public static class ArticleMappings
{
    public static readonly Expression<Func<Article, ArticleDto>> ToDto = a => new ArticleDto(
        a.Id, a.Title, a.Content, a.Category,
        a.CoverImage != null ? a.CoverImage.Url : null,
        a.Author, a.PublishedAt, a.CoverImageId);
}

public static class VideoMappings
{
    public static readonly Expression<Func<Video, VideoDto>> ToDto = v => new VideoDto(
        v.Id, v.Title, v.Description, v.VideoFile.Url,
        v.ThumbnailImage != null ? v.ThumbnailImage.Url : null,
        v.DurationSeconds, v.Category, v.VideoFileId, v.ThumbnailImageId);
}
