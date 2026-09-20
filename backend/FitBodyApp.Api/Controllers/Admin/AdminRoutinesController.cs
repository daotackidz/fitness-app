using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Workout;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/routines")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminRoutinesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminRoutinesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Routines.Include(r => r.ImageFile).OrderBy(r => r.Name)
            .Select(r => new RoutineDto(r.Id, r.Name, r.Level.ToString(), r.Description, r.DurationWeeks, r.IsCustom,
                r.CreatedByUserId, r.ImageFile != null ? r.ImageFile.Url : null, r.DurationMinutes, r.CaloriesEstimate,
                r.RoutineExercises.Count, r.IsFeatured, false, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<RoutineDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertRoutineRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("level không hợp lệ");

        var routine = new Routine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Level = level,
            Description = request.Description,
            DurationWeeks = request.DurationWeeks,
            IsCustom = false,
            ImageFileId = request.ImageFileId,
            DurationMinutes = request.DurationMinutes,
            CaloriesEstimate = request.CaloriesEstimate,
            IsFeatured = request.IsFeatured
        };
        _db.Routines.Add(routine);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = routine.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertRoutineRequest request)
    {
        var routine = await _db.Routines.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy routine");
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("level không hợp lệ");

        routine.Name = request.Name;
        routine.Level = level;
        routine.Description = request.Description;
        routine.DurationWeeks = request.DurationWeeks;
        routine.ImageFileId = request.ImageFileId;
        routine.DurationMinutes = request.DurationMinutes;
        routine.CaloriesEstimate = request.CaloriesEstimate;
        routine.IsFeatured = request.IsFeatured;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var routine = await _db.Routines.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy routine");
        _db.Routines.Remove(routine);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
