using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/exercises")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminExercisesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminExercisesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Exercises.OrderBy(e => e.Name)
            .Select(e => new ExerciseDto(e.Id, e.Name, e.Description, e.MuscleGroup, e.Equipment, e.DifficultyLevel.ToString(), e.VideoUrl, e.ImageUrl, e.CaloriesEstimate))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<ExerciseDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertExerciseRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.DifficultyLevel, true, out var level))
            throw AppException.ValidationError("difficulty_level khong hop le");

        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            MuscleGroup = request.MuscleGroup,
            Equipment = request.Equipment,
            DifficultyLevel = level,
            VideoUrl = request.VideoUrl,
            ImageUrl = request.ImageUrl,
            CaloriesEstimate = request.CaloriesEstimate
        };
        _db.Exercises.Add(exercise);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = exercise.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertExerciseRequest request)
    {
        var exercise = await _db.Exercises.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay bai tap");
        if (!Enum.TryParse<DifficultyLevel>(request.DifficultyLevel, true, out var level))
            throw AppException.ValidationError("difficulty_level khong hop le");

        exercise.Name = request.Name;
        exercise.Description = request.Description;
        exercise.MuscleGroup = request.MuscleGroup;
        exercise.Equipment = request.Equipment;
        exercise.DifficultyLevel = level;
        exercise.VideoUrl = request.VideoUrl;
        exercise.ImageUrl = request.ImageUrl;
        exercise.CaloriesEstimate = request.CaloriesEstimate;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat thanh cong" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var exercise = await _db.Exercises.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay bai tap");
        _db.Exercises.Remove(exercise);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
