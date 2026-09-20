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

    [HttpGet("user-growth")]
    public async Task<IActionResult> GetUserGrowth([FromQuery] int days = 30)
    {
        days = days is < 1 or > 365 ? 30 : days;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(-(days - 1));
        var startDateTime = DateTime.SpecifyKind(startDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var raw = await _db.Users
            .Where(u => u.CreatedAt >= startDateTime)
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        var counts = raw.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);
        return Ok(ApiResponse<List<TimeSeriesPointDto>>.Ok(BuildSeries(startDate, days, counts)));
    }

    [HttpGet("workout-activity")]
    public async Task<IActionResult> GetWorkoutActivity([FromQuery] int days = 30)
    {
        days = days is < 1 or > 365 ? 30 : days;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(-(days - 1));

        var raw = await _db.WorkoutLogs
            .Where(w => w.LogDate >= startDate)
            .GroupBy(w => w.LogDate)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        var counts = raw.ToDictionary(x => x.Date, x => x.Count);
        return Ok(ApiResponse<List<TimeSeriesPointDto>>.Ok(BuildSeries(startDate, days, counts)));
    }

    [HttpGet("content-distribution")]
    public async Task<IActionResult> GetContentDistribution()
    {
        var items = new List<ContentDistributionDto>
        {
            new("Exercises", await _db.Exercises.CountAsync()),
            new("Routines", await _db.Routines.CountAsync()),
            new("Meal Plans", await _db.MealPlans.CountAsync()),
            new("Articles", await _db.Articles.CountAsync()),
            new("Videos", await _db.Videos.CountAsync()),
            new("Faqs", await _db.Faqs.CountAsync()),
            new("Challenges", await _db.Challenges.CountAsync())
        };
        return Ok(ApiResponse<List<ContentDistributionDto>>.Ok(items));
    }

    [HttpGet("ticket-status")]
    public async Task<IActionResult> GetTicketStatus()
    {
        var items = await _db.SupportTickets
            .GroupBy(t => t.Status)
            .Select(g => new ContentDistributionDto(g.Key.ToString(), g.Count()))
            .ToListAsync();
        return Ok(ApiResponse<List<ContentDistributionDto>>.Ok(items));
    }

    [HttpGet("report-status")]
    public async Task<IActionResult> GetReportStatus()
    {
        var items = await _db.ReportedContents
            .GroupBy(r => r.Status)
            .Select(g => new ContentDistributionDto(g.Key.ToString(), g.Count()))
            .ToListAsync();
        return Ok(ApiResponse<List<ContentDistributionDto>>.Ok(items));
    }

    private static List<TimeSeriesPointDto> BuildSeries(DateOnly start, int days, Dictionary<DateOnly, int> counts)
    {
        var result = new List<TimeSeriesPointDto>(days);
        for (var i = 0; i < days; i++)
        {
            var date = start.AddDays(i);
            result.Add(new TimeSeriesPointDto(date, counts.TryGetValue(date, out var c) ? c : 0));
        }
        return result;
    }
}
