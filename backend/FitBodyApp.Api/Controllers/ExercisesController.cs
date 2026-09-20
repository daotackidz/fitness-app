using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/exercises")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public ExercisesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? difficulty, [FromQuery] string? muscleGroup, [FromQuery] string? q,
        [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Exercises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(difficulty) && Enum.TryParse<DifficultyLevel>(difficulty, true, out var level))
            query = query.Where(e => e.DifficultyLevel == level);

        if (!string.IsNullOrWhiteSpace(muscleGroup))
            query = query.Where(e => e.MuscleGroup == muscleGroup);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{q}%"));

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(e => e.Name)
            .Select(ExerciseMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ExerciseDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var dto = await _db.Exercises.Where(e => e.Id == id).Select(ExerciseMappings.ToDto).FirstOrDefaultAsync();
        if (dto is null) throw AppException.NotFound("Không tìm thấy bài tập");

        return Ok(ApiResponse<ExerciseDto>.Ok(dto));
    }
}
