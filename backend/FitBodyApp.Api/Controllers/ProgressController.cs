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
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public ProgressController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProgressRequest request)
    {
        var record = new ProgressTracking
        {
            Id = Guid.NewGuid(),
            UserId = User.GetUserId(),
            RecordDate = request.RecordDate,
            WeightKg = request.WeightKg,
            BodyFatPercent = request.BodyFatPercent,
            MeasurementsJson = request.MeasurementsJson,
            PhotoUrl = request.PhotoUrl
        };
        _db.ProgressTrackings.Add(record);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = record.Id }));
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = _db.ProgressTrackings.Where(p => p.UserId == userId);

        if (from is not null) query = query.Where(p => p.RecordDate >= from);
        if (to is not null) query = query.Where(p => p.RecordDate <= to);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(p => p.RecordDate)
            .Select(p => new ProgressDto(p.Id, p.RecordDate, p.WeightKg, p.BodyFatPercent, p.MeasurementsJson, p.PhotoUrl))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ProgressDto>>.Ok(items, meta));
    }
}
