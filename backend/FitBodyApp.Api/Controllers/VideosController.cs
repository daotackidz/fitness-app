using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/videos")]
[Authorize]
public class VideosController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public VideosController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Videos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(v => v.Category == category);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(v => v.Title)
            .Select(v => new VideoDto(v.Id, v.Title, v.Description, v.VideoUrl, v.ThumbnailUrl, v.DurationSeconds, v.Category))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<VideoDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var video = await _db.Videos.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay video");
        var dto = new VideoDto(video.Id, video.Title, video.Description, video.VideoUrl, video.ThumbnailUrl, video.DurationSeconds, video.Category);
        return Ok(ApiResponse<VideoDto>.Ok(dto));
    }
}
