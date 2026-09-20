using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Support;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public NotificationsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Notifications.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.Type.ToString(), n.Title, n.Message, n.IsRead, n.ScheduledAt, n.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<NotificationDto>>.Ok(items, meta));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateReadStatus(Guid id, UpdateNotificationRequest request)
    {
        var userId = User.GetUserId();
        var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId)
            ?? throw AppException.NotFound("Không tìm thấy thông báo");

        notification.IsRead = request.IsRead;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }
}
