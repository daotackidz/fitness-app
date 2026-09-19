namespace FitBodyApp.Application.Common;

public record BlobUploadResult(string Url, string BlobName, string Container, string FileName, string? ContentType, long SizeBytes);

public interface IBlobStorageService
{
    Task<BlobUploadResult> UploadAsync(string containerName, Stream content, string fileName, string? contentType);
}

public interface IMediaFileService
{
    Task<(Guid Id, string Url)> SaveVideoFileAsync(BlobUploadResult upload, Guid? uploadedByUserId);
    Task<(Guid Id, string Url)> SaveImageFileAsync(BlobUploadResult upload, Guid? uploadedByUserId);
}

public static class BlobContainers
{
    public const string Avatars = "avatars";
    public const string Exercises = "exercises";
    public const string Articles = "articles";
    public const string Videos = "videos";
    public const string MealPlans = "meal-plans";
    public const string ForumPosts = "forum-posts";
    public const string Progress = "progress";

    public static readonly HashSet<string> AdminAllowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Exercises, Articles, Videos, MealPlans
    };

    public static readonly HashSet<string> UserAllowed = new(StringComparer.OrdinalIgnoreCase)
    {
        ForumPosts, Progress
    };
}

public enum MediaKind { Video, Image }
