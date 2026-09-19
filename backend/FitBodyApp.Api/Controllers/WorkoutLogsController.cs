using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/workout-logs")]
[Authorize]
public class WorkoutLogsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public WorkoutLogsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkoutLogRequest request)
    {
        var log = new WorkoutLog
        {
            Id = Guid.NewGuid(),
            UserId = User.GetUserId(),
            RoutineId = request.RoutineId,
            ExerciseId = request.ExerciseId,
            LogDate = request.LogDate,
            DurationMinutes = request.DurationMinutes,
            SetsCompleted = request.SetsCompleted,
            RepsCompleted = request.RepsCompleted,
            WeightUsedKg = request.WeightUsedKg,
            CaloriesBurned = request.CaloriesBurned
        };
        _db.WorkoutLogs.Add(log);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = log.Id }));
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = _db.WorkoutLogs.Where(w => w.UserId == userId);

        if (from is not null) query = query.Where(w => w.LogDate >= from);
        if (to is not null) query = query.Where(w => w.LogDate <= to);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(w => w.LogDate)
            .Select(w => new WorkoutLogDto(w.Id, w.RoutineId, w.ExerciseId, w.LogDate, w.DurationMinutes,
                w.SetsCompleted, w.RepsCompleted, w.WeightUsedKg, w.CaloriesBurned))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<WorkoutLogDto>>.Ok(items, meta));
    }
}
