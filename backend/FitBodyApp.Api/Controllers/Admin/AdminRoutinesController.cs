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
        var (items, meta) = await _db.Routines.OrderBy(r => r.Name)
            .Select(r => new RoutineDto(r.Id, r.Name, r.Level.ToString(), r.Description, r.DurationWeeks, r.IsCustom, r.CreatedByUserId, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<RoutineDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertRoutineRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("level khong hop le");

        var routine = new Routine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Level = level,
            Description = request.Description,
            DurationWeeks = request.DurationWeeks,
            IsCustom = false
        };
        _db.Routines.Add(routine);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = routine.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertRoutineRequest request)
    {
        var routine = await _db.Routines.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay routine");
        if (!Enum.TryParse<DifficultyLevel>(request.Level, true, out var level))
            throw AppException.ValidationError("level khong hop le");

        routine.Name = request.Name;
        routine.Level = level;
        routine.Description = request.Description;
        routine.DurationWeeks = request.DurationWeeks;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat thanh cong" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var routine = await _db.Routines.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay routine");
        _db.Routines.Remove(routine);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
