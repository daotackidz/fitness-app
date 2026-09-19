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

    public UploadsController(IBlobStorageService blobStorage)
    {
        _blobStorage = blobStorage;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string container)
    {
        if (!BlobContainers.UserAllowed.Contains(container))
            throw AppException.ValidationError($"Container khong hop le. Cho phep: {string.Join(", ", BlobContainers.UserAllowed)}");

        await using var stream = file.OpenReadStream();
        var url = await _blobStorage.UploadAsync(container, stream, file.FileName, file.ContentType);

        return Ok(ApiResponse<object>.Ok(new { url }));
    }
}
