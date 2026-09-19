using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminModerationController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminModerationController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/admin/reported-contents")]
    public async Task<IActionResult> GetReports([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.ReportedContents.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ReportStatus>(status, true, out var s))
            query = query.Where(r => r.Status == s);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReportedContentDto(r.Id, r.ReporterUserId, r.ReportableType.ToString(), r.ReportableId, r.Reason, r.Status.ToString(), r.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ReportedContentDto>>.Ok(items, meta));
    }

    [HttpPatch("api/admin/reported-contents/{id:guid}")]
    public async Task<IActionResult> UpdateReportStatus(Guid id, UpdateReportedContentStatusRequest request)
    {
        if (!Enum.TryParse<ReportStatus>(request.Status, true, out var status))
            throw AppException.ValidationError("status khong hop le");

        var report = await _db.ReportedContents.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay bao cao");
        report.Status = status;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat trang thai thanh cong" }));
    }

    [HttpDelete("api/admin/forum-posts/{id:guid}")]
    public async Task<IActionResult> DeleteForumPost(Guid id)
    {
        var post = await _db.ForumPosts.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay bai dang");
        _db.ForumPosts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/admin/comments/{id:guid}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var comment = await _db.Comments.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay binh luan");
        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
