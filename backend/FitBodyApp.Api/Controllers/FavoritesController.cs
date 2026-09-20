using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public FavoritesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFavoriteRequest request)
    {
        if (!Enum.TryParse<FavoritableType>(request.FavoritableType, true, out var type))
            throw AppException.ValidationError("favoritable_type không hợp lệ");

        var exists = type switch
        {
            FavoritableType.Article => await _db.Articles.AnyAsync(a => a.Id == request.FavoritableId),
            FavoritableType.Video => await _db.Videos.AnyAsync(v => v.Id == request.FavoritableId),
            FavoritableType.Routine => await _db.Routines.AnyAsync(r => r.Id == request.FavoritableId),
            _ => await _db.Exercises.AnyAsync(e => e.Id == request.FavoritableId)
        };
        if (!exists)
            throw AppException.NotFound("Không tìm thấy nội dung để yêu thích");

        var userId = User.GetUserId();
        var already = await _db.Favorites.AnyAsync(f => f.UserId == userId && f.FavoritableType == type && f.FavoritableId == request.FavoritableId);
        if (already)
            throw AppException.Conflict("Đã yêu thích nội dung này rồi");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FavoritableType = type,
            FavoritableId = request.FavoritableId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Favorites.Add(favorite);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = favorite.Id }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId)
            ?? throw AppException.NotFound("Không tìm thấy mục yêu thích");

        _db.Favorites.Remove(favorite);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = _db.Favorites.Where(f => f.UserId == userId);

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<FavoritableType>(type, true, out var favType))
            query = query.Where(f => f.FavoritableType == favType);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (favorites, meta) = await query.OrderByDescending(f => f.CreatedAt).ToPagedResultAsync(paging.Page, paging.Limit);

        var articleIds = favorites.Where(f => f.FavoritableType == FavoritableType.Article).Select(f => f.FavoritableId).ToList();
        var videoIds = favorites.Where(f => f.FavoritableType == FavoritableType.Video).Select(f => f.FavoritableId).ToList();
        var exerciseIds = favorites.Where(f => f.FavoritableType == FavoritableType.Exercise).Select(f => f.FavoritableId).ToList();
        var routineIds = favorites.Where(f => f.FavoritableType == FavoritableType.Routine).Select(f => f.FavoritableId).ToList();

        var articles = await _db.Articles.Where(a => articleIds.Contains(a.Id))
            .Select(a => new { a.Id, a.Title, ImageUrl = a.CoverImage != null ? a.CoverImage.Url : null, a.Content })
            .ToDictionaryAsync(a => a.Id);
        var videos = await _db.Videos.Where(v => videoIds.Contains(v.Id))
            .Select(v => new { v.Id, v.Title, ImageUrl = v.ThumbnailImage != null ? v.ThumbnailImage.Url : null, v.Description, v.DurationSeconds, v.CaloriesEstimate, v.ExerciseCount })
            .ToDictionaryAsync(v => v.Id);
        var exercises = await _db.Exercises.Where(e => exerciseIds.Contains(e.Id))
            .Select(e => new { e.Id, e.Name, ImageUrl = e.ImageFile != null ? e.ImageFile.Url : null, e.CaloriesEstimate, e.DurationMinutes })
            .ToDictionaryAsync(e => e.Id);
        var routines = await _db.Routines.Where(r => routineIds.Contains(r.Id))
            .Select(r => new { r.Id, r.Name, ImageUrl = r.ImageFile != null ? r.ImageFile.Url : null, r.CaloriesEstimate, r.DurationMinutes, ExerciseCount = r.RoutineExercises.Count })
            .ToDictionaryAsync(r => r.Id);

        var items = new List<FavoriteDetailedDto>();
        foreach (var f in favorites)
        {
            if (f.FavoritableType == FavoritableType.Article && articles.TryGetValue(f.FavoritableId, out var a))
                items.Add(new FavoriteDetailedDto(f.Id, "Article", f.FavoritableId, a.Title, a.ImageUrl, a.Content, null, null, null, f.CreatedAt));
            else if (f.FavoritableType == FavoritableType.Video && videos.TryGetValue(f.FavoritableId, out var v))
                items.Add(new FavoriteDetailedDto(f.Id, "Video", f.FavoritableId, v.Title, v.ImageUrl, v.Description,
                    v.DurationSeconds.HasValue ? v.DurationSeconds.Value / 60 : null, v.CaloriesEstimate, v.ExerciseCount, f.CreatedAt));
            else if (f.FavoritableType == FavoritableType.Exercise && exercises.TryGetValue(f.FavoritableId, out var e))
                items.Add(new FavoriteDetailedDto(f.Id, "Exercise", f.FavoritableId, e.Name, e.ImageUrl, null, e.DurationMinutes, e.CaloriesEstimate, null, f.CreatedAt));
            else if (f.FavoritableType == FavoritableType.Routine && routines.TryGetValue(f.FavoritableId, out var r))
                items.Add(new FavoriteDetailedDto(f.Id, "Routine", f.FavoritableId, r.Name, r.ImageUrl, null, r.DurationMinutes, r.CaloriesEstimate, r.ExerciseCount, f.CreatedAt));
        }

        return Ok(ApiResponse<List<FavoriteDetailedDto>>.Ok(items, meta));
    }
}
