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
        if (!string.IsNullOrWhiteSpace(level) && Enum.TryParse<DifficultyLevel>(level, true, out var lvl))
            query = query.Where(r => r.Level == lvl);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(r => r.Name)
            .Select(r => new RoutineDto(r.Id, r.Name, r.Level.ToString(), r.Description, r.DurationWeeks, r.IsCustom, r.CreatedByUserId, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<RoutineDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var routine = await _db.Routines
            .Include(r => r.RoutineExercises).ThenInclude(re => re.Exercise)
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw AppException.NotFound("Khong tim thay routine");

        var exercises = routine.RoutineExercises.OrderBy(re => re.OrderIndex)
            .Select(re => new RoutineExerciseDto(re.ExerciseId, re.Exercise.Name, re.Sets, re.Reps, re.RestSeconds, re.OrderIndex))
            .ToList();

        var dto = new RoutineDto(routine.Id, routine.Name, routine.Level.ToString(), routine.Description,
            routine.DurationWeeks, routine.IsCustom, routine.CreatedByUserId, exercises);
        return Ok(ApiResponse<RoutineDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoutineRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("Level khong hop le");

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
                OrderIndex = item.OrderIndex
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
            ?? throw AppException.NotFound("Khong tim thay routine");

        if (routine.CreatedByUserId != userId)
            throw AppException.Forbidden("Ban khong co quyen sua routine nay");

        if (request.Name is not null) routine.Name = request.Name;
        if (request.Description is not null) routine.Description = request.Description;
        if (request.DurationWeeks is not null) routine.DurationWeeks = request.DurationWeeks;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat routine thanh cong" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var routine = await _db.Routines.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw AppException.NotFound("Khong tim thay routine");

        if (routine.CreatedByUserId != userId)
            throw AppException.Forbidden("Ban khong co quyen xoa routine nay");

        _db.Routines.Remove(routine);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
