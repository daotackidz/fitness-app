using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/routines")]
[Authorize]
public class RoutinesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public RoutinesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? level, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Routines.AsQueryable();
        if (!string.IsNullOrWhiteSpace(level))
        {
            var levels = level.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(l => Enum.TryParse<DifficultyLevel>(l, true, out var lvl) ? (DifficultyLevel?)lvl : null)
                .Where(l => l is not null).Select(l => l!.Value).ToList();
            if (levels.Count > 0) query = query.Where(r => levels.Contains(r.Level));
        }

        var userId = User.GetUserId();
        var favoriteIds = await _db.Favorites
            .Where(f => f.UserId == userId && f.FavoritableType == FavoritableType.Routine)
            .Select(f => f.FavoritableId).ToListAsync();

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.Include(r => r.ImageFile).OrderByDescending(r => r.IsFeatured).ThenBy(r => r.Name)
            .Select(r => new RoutineDto(r.Id, r.Name, r.Level.ToString(), r.Description, r.DurationWeeks, r.IsCustom,
                r.CreatedByUserId, r.ImageFile != null ? r.ImageFile.Url : null, r.DurationMinutes, r.CaloriesEstimate,
                r.RoutineExercises.Count, r.IsFeatured, favoriteIds.Contains(r.Id), null))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<RoutineDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var userId = User.GetUserId();
        var routine = await _db.Routines
            .Include(r => r.ImageFile)
            .Include(r => r.RoutineExercises).ThenInclude(re => re.Exercise).ThenInclude(e => e.ImageFile)
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw AppException.NotFound("Không tìm thấy routine");

        var isFavorited = await _db.Favorites.AnyAsync(f => f.UserId == userId && f.FavoritableType == FavoritableType.Routine && f.FavoritableId == id);

        var exercises = routine.RoutineExercises.OrderBy(re => re.RoundNumber).ThenBy(re => re.OrderIndex)
            .Select(re => new RoutineExerciseDto(re.ExerciseId, re.Exercise.Name, re.Sets, re.Reps, re.RestSeconds, re.OrderIndex,
                re.RoundNumber, re.DurationSeconds, re.Exercise.ImageFile != null ? re.Exercise.ImageFile.Url : null,
                re.Exercise.Description, re.Exercise.CaloriesEstimate, re.Exercise.DifficultyLevel.ToString()))
            .ToList();

        var dto = new RoutineDto(routine.Id, routine.Name, routine.Level.ToString(), routine.Description,
            routine.DurationWeeks, routine.IsCustom, routine.CreatedByUserId,
            routine.ImageFile != null ? routine.ImageFile.Url : null, routine.DurationMinutes, routine.CaloriesEstimate,
            exercises.Count, routine.IsFeatured, isFavorited, exercises);
        return Ok(ApiResponse<RoutineDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoutineRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("Level không hợp lệ");

        var userId = User.GetUserId();
        var routine = new Routine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Level = level,
            Description = request.Description,
            DurationWeeks = request.DurationWeeks,
            IsCustom = true,
            CreatedByUserId = userId
        };
        _db.Routines.Add(routine);

        foreach (var item in request.Exercises)
        {
            _db.RoutineExercises.Add(new RoutineExercise
            {
                Id = Guid.NewGuid(),
                RoutineId = routine.Id,
                ExerciseId = item.ExerciseId,
                Sets = item.Sets,
                Reps = item.Reps,
                RestSeconds = item.RestSeconds,
                OrderIndex = item.OrderIndex,
                RoundNumber = item.RoundNumber,
                DurationSeconds = item.DurationSeconds
            });
        }
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetail), new { id = routine.Id },
            ApiResponse<object>.Ok(new { id = routine.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoutineRequest request)
    {
        var userId = User.GetUserId();
        var routine = await _db.Routines.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw AppException.NotFound("Không tìm thấy routine");

        if (routine.CreatedByUserId != userId)
            throw AppException.Forbidden("Bạn không có quyền sửa routine này");

        if (request.Name is not null) routine.Name = request.Name;
        if (request.Description is not null) routine.Description = request.Description;
        if (request.DurationWeeks is not null) routine.DurationWeeks = request.DurationWeeks;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật routine thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var routine = await _db.Routines.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw AppException.NotFound("Không tìm thấy routine");

        if (routine.CreatedByUserId != userId)
            throw AppException.Forbidden("Bạn không có quyền xóa routine này");

        _db.Routines.Remove(routine);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
