using FitBodyApp.Api.Hubs;
using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Support;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/support/tickets")]
[Authorize]
public class SupportController : ControllerBase
{
    private readonly FitBodyDbContext _db;
    private readonly IHubContext<SupportHub> _hub;

    public SupportController(FitBodyDbContext db, IHubContext<SupportHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSupportTicketRequest request)
    {
        var ticket = new SupportTicket
        {
            Id = Guid.NewGuid(),
            UserId = User.GetUserId(),
            Subject = request.Subject,
            Status = SupportTicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
        _db.SupportTickets.Add(ticket);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = ticket.Id }));
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.SupportTickets.Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new SupportTicketDto(t.Id, t.Subject, t.Status.ToString(), t.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<SupportTicketDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int page = 1, [FromQuery] int limit = 50)
    {
        var userId = User.GetUserId();
        var ownsTicket = await _db.SupportTickets.AnyAsync(t => t.Id == id && t.UserId == userId);
        if (!ownsTicket) throw AppException.Forbidden("Bạn không có quyền xem ticket này");

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.SupportMessages.Where(m => m.TicketId == id)
            .OrderBy(m => m.SentAt)
            .Select(m => new SupportMessageDto(m.Id, m.TicketId, m.SenderType.ToString(), m.Message, m.SentAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<SupportMessageDto>>.Ok(items, meta));
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, CreateSupportMessageRequest request)
    {
        var userId = User.GetUserId();
        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId)
            ?? throw AppException.Forbidden("Bạn không có quyền gửi tin nhắn trong ticket này");

        var message = new SupportMessage
        {
            Id = Guid.NewGuid(),
            TicketId = id,
            SenderType = SupportSenderType.User,
            Message = request.Message,
            SentAt = DateTime.UtcNow
        };
        _db.SupportMessages.Add(message);
        await _db.SaveChangesAsync();

        var dto = new SupportMessageDto(message.Id, message.TicketId, message.SenderType.ToString(), message.Message, message.SentAt);
        await _hub.Clients.Group(id.ToString()).SendAsync("ReceiveMessage", dto);

        return CreatedAtAction(nameof(GetMessages), new { id }, ApiResponse<SupportMessageDto>.Ok(dto));
    }
}
