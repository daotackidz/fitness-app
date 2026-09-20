using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IMediaFileService _mediaFileService;

    public UploadsController(IBlobStorageService blobStorage, IMediaFileService mediaFileService)
    {
        _blobStorage = blobStorage;
        _mediaFileService = mediaFileService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string container, [FromQuery] string type)
    {
        if (!BlobContainers.UserAllowed.Contains(container))
            throw AppException.ValidationError($"Container không hợp lệ. Cho phép: {string.Join(", ", BlobContainers.UserAllowed)}");

        if (!Enum.TryParse<MediaKind>(type, true, out var mediaKind))
            throw AppException.ValidationError("type phải là 'video' hoặc 'image'");

        await using var stream = file.OpenReadStream();
        var uploadResult = await _blobStorage.UploadAsync(container, stream, file.FileName, file.ContentType);
        var userId = User.GetUserId();

        var (id, url) = mediaKind == MediaKind.Video
            ? await _mediaFileService.SaveVideoFileAsync(uploadResult, userId)
            : await _mediaFileService.SaveImageFileAsync(uploadResult, userId);

        return Ok(ApiResponse<object>.Ok(new { id, url }));
    }
}
