using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/recommendations")]
[Authorize]
public class RecommendationsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public RecommendationsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var userId = User.GetUserId();
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");

        var recentExerciseIds = await _db.WorkoutLogs
            .Where(w => w.UserId == userId && w.ExerciseId != null && w.LogDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)))
            .Select(w => w.ExerciseId!.Value)
            .Distinct()
            .ToListAsync();

        var targetLevel = user.ActivityLevel switch
        {
            ActivityLevel.Active or ActivityLevel.VeryActive => DifficultyLevel.Advanced,
            ActivityLevel.Moderate => DifficultyLevel.Intermediate,
            _ => DifficultyLevel.Beginner
        };

        var suggestions = await _db.Exercises
            .Where(e => e.DifficultyLevel == targetLevel && !recentExerciseIds.Contains(e.Id))
            .OrderBy(e => e.Name)
            .Take(5)
            .Select(e => new RecommendationDto(e.Id, e.Name, $"Phu hop voi muc do van dong '{targetLevel}' cua ban"))
            .ToListAsync();

        return Ok(ApiResponse<List<RecommendationDto>>.Ok(suggestions));
    }
}
