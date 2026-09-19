using FitBodyApp.Application.Common;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;

namespace FitBodyApp.Infrastructure.Storage;

public class MediaFileService : IMediaFileService
{
    private readonly FitBodyDbContext _db;

    public MediaFileService(FitBodyDbContext db)
    {
        _db = db;
    }

    public async Task<(Guid Id, string Url)> SaveVideoFileAsync(BlobUploadResult upload, Guid? uploadedByUserId)
    {
        var entity = new VideoFile
        {
            Id = Guid.NewGuid(),
            Url = upload.Url,
            BlobName = upload.BlobName,
            Container = upload.Container,
            FileName = upload.FileName,
            ContentType = upload.ContentType,
            SizeBytes = upload.SizeBytes,
            UploadedByUserId = uploadedByUserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.VideoFiles.Add(entity);
        await _db.SaveChangesAsync();
        return (entity.Id, entity.Url);
    }

    public async Task<(Guid Id, string Url)> SaveImageFileAsync(BlobUploadResult upload, Guid? uploadedByUserId)
    {
        var entity = new ImageFile
        {
            Id = Guid.NewGuid(),
            Url = upload.Url,
            BlobName = upload.BlobName,
            Container = upload.Container,
            FileName = upload.FileName,
            ContentType = upload.ContentType,
            SizeBytes = upload.SizeBytes,
            UploadedByUserId = uploadedByUserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.ImageFiles.Add(entity);
        await _db.SaveChangesAsync();
        return (entity.Id, entity.Url);
    }
}
