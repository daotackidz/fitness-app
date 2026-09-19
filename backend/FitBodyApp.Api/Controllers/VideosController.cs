using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            .Select(VideoMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<VideoDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var dto = await _db.Videos.Where(v => v.Id == id).Select(VideoMappings.ToDto).FirstOrDefaultAsync();
        if (dto is null) throw AppException.NotFound("Khong tim thay video");
        return Ok(ApiResponse<VideoDto>.Ok(dto));
    }
}
