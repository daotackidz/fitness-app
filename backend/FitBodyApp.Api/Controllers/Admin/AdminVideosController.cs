using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/videos")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminVideosController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminVideosController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Videos.OrderBy(v => v.Title)
            .Select(VideoMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<VideoDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertVideoRequest request)
    {
        var video = new Video
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            VideoFileId = request.VideoFileId,
            ThumbnailImageId = request.ThumbnailImageId,
            DurationSeconds = request.DurationSeconds,
            Category = request.Category
        };
        _db.Videos.Add(video);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = video.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertVideoRequest request)
    {
        var video = await _db.Videos.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay video");
        video.Title = request.Title;
        video.Description = request.Description;
        video.VideoFileId = request.VideoFileId;
        video.ThumbnailImageId = request.ThumbnailImageId;
        video.DurationSeconds = request.DurationSeconds;
        video.Category = request.Category;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat thanh cong" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var video = await _db.Videos.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay video");
        _db.Videos.Remove(video);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
