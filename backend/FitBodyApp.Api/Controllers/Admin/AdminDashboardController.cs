using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminDashboardController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminDashboardController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var weekStart = todayStart.AddDays(-(int)now.DayOfWeek);

        var totalUsers = await _db.Users.CountAsync();
        var newUsersToday = await _db.Users.CountAsync(u => u.CreatedAt >= todayStart);
        var newUsersThisWeek = await _db.Users.CountAsync(u => u.CreatedAt >= weekStart);
        var openTickets = await _db.SupportTickets.CountAsync(t => t.Status == SupportTicketStatus.Open);
        var pendingReports = await _db.ReportedContents.CountAsync(r => r.Status == ReportStatus.Pending);

        var topRoutines = await _db.WorkoutLogs
            .Where(w => w.RoutineId != null)
            .GroupBy(w => w.RoutineId!.Value)
            .Select(g => new { RoutineId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .Join(_db.Routines, g => g.RoutineId, r => r.Id, (g, r) => new TopContentDto(r.Id, r.Name, g.Count))
            .ToListAsync();

        var topArticles = await _db.Favorites
            .Where(f => f.FavoritableType == FavoritableType.Article)
            .GroupBy(f => f.FavoritableId)
            .Select(g => new { ArticleId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .Join(_db.Articles, g => g.ArticleId, a => a.Id, (g, a) => new TopContentDto(a.Id, a.Title, g.Count))
            .ToListAsync();

        var summary = new DashboardSummaryDto(totalUsers, newUsersToday, newUsersThisWeek, openTickets, pendingReports, topRoutines, topArticles);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(summary));
    }
}
