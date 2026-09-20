using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Community;
using FitBodyApp.Application.Home;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/home")]
[Authorize]
public class HomeController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public HomeController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.GetUserId();
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy người dùng");

        var favoriteExerciseIds = await _db.Favorites
            .Where(f => f.UserId == userId && f.FavoritableType == FavoritableType.Exercise)
            .Select(f => f.FavoritableId).ToListAsync();
        var favoriteArticleIds = await _db.Favorites
            .Where(f => f.UserId == userId && f.FavoritableType == FavoritableType.Article)
            .Select(f => f.FavoritableId).ToListAsync();

        var recentExerciseIds = await _db.WorkoutLogs
            .Where(w => w.UserId == userId && w.ExerciseId != null && w.LogDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)))
            .Select(w => w.ExerciseId!.Value).Distinct().ToListAsync();

        var targetLevel = user.ActivityLevel switch
        {
            ActivityLevel.Advanced => DifficultyLevel.Advanced,
            ActivityLevel.Intermediate => DifficultyLevel.Intermediate,
            _ => DifficultyLevel.Beginner
        };

        var recommendations = await _db.Exercises
            .Where(e => e.DifficultyLevel == targetLevel && !recentExerciseIds.Contains(e.Id))
            .OrderBy(e => e.Name)
            .Take(4)
            .Select(e => new HomeExerciseCardDto(e.Id, e.Name, e.ImageFile != null ? e.ImageFile.Url : null,
                e.DurationMinutes, e.CaloriesEstimate, favoriteExerciseIds.Contains(e.Id)))
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var challenge = await _db.Challenges
            .Include(c => c.ImageFile)
            .Where(c => c.Type == ChallengeType.Weekly && c.StartDate <= today && c.EndDate >= today)
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefaultAsync();
        var featuredChallenge = challenge is null ? null : new ChallengeDto(challenge.Id, challenge.Name, challenge.Description,
            challenge.Type.ToString(), challenge.StartDate, challenge.EndDate, challenge.GoalMetric, challenge.Reward,
            challenge.ImageFile != null ? challenge.ImageFile.Url : null, null);

        var articles = await _db.Articles
            .OrderByDescending(a => a.PublishedAt)
            .Take(2)
            .Select(a => new HomeArticleCardDto(a.Id, a.Title, a.CoverImage != null ? a.CoverImage.Url : null,
                favoriteArticleIds.Contains(a.Id)))
            .ToListAsync();

        var dto = new HomeDto(user.FullName, recommendations, featuredChallenge, articles);
        return Ok(ApiResponse<HomeDto>.Ok(dto));
    }
}
