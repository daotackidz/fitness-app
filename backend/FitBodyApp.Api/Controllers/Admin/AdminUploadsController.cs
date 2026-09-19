using FitBodyApp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/uploads")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminUploadsController : ControllerBase
{
    private readonly IBlobStorageService _blobStorage;

    public AdminUploadsController(IBlobStorageService blobStorage)
    {
        _blobStorage = blobStorage;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string container)
    {
        if (!BlobContainers.AdminAllowed.Contains(container))
            throw AppException.ValidationError($"Container khong hop le. Cho phep: {string.Join(", ", BlobContainers.AdminAllowed)}");

        await using var stream = file.OpenReadStream();
        var url = await _blobStorage.UploadAsync(container, stream, file.FileName, file.ContentType);

        return Ok(ApiResponse<object>.Ok(new { url }));
    }
}
