namespace FitBodyApp.Domain.Entities;

public class VideoFile
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
    public Guid? UploadedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? UploadedByUser { get; set; }
}

public class ImageFile
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
    public Guid? UploadedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? UploadedByUser { get; set; }
}
