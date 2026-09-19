using FitBodyApp.Api.Hubs;
using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Support;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/support-tickets")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminSupportController : ControllerBase
{
    private readonly FitBodyDbContext _db;
    private readonly IHubContext<SupportHub> _hub;

    public AdminSupportController(FitBodyDbContext db, IHubContext<SupportHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.SupportTickets.Include(t => t.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SupportTicketStatus>(status, true, out var st))
            query = query.Where(t => t.Status == st);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(t => t.CreatedAt)
            .Select(t => new AdminSupportTicketDto(t.Id, t.UserId, t.User.FullName, t.Subject, t.Status.ToString(), t.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<AdminSupportTicketDto>>.Ok(items, meta));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTicketStatusRequest request)
    {
        if (!Enum.TryParse<SupportTicketStatus>(request.Status, true, out var status))
            throw AppException.ValidationError("status khong hop le");

        var ticket = await _db.SupportTickets.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay ticket");
        ticket.Status = status;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat trang thai thanh cong" }));
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> Reply(Guid id, CreateSupportMessageRequest request)
    {
        var exists = await _db.SupportTickets.AnyAsync(t => t.Id == id);
        if (!exists) throw AppException.NotFound("Khong tim thay ticket");

        var message = new SupportMessage
        {
            Id = Guid.NewGuid(),
            TicketId = id,
            SenderType = SupportSenderType.Agent,
            Message = request.Message,
            SentAt = DateTime.UtcNow
        };
        _db.SupportMessages.Add(message);
        await _db.SaveChangesAsync();

        var dto = new SupportMessageDto(message.Id, message.TicketId, message.SenderType.ToString(), message.Message, message.SentAt);
        await _hub.Clients.Group(id.ToString()).SendAsync("ReceiveMessage", dto);

        return CreatedAtAction(nameof(GetList), null, ApiResponse<SupportMessageDto>.Ok(dto));
    }
}
