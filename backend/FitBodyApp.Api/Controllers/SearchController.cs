using FitBodyApp.Application.Common;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public SearchController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] string type = "all")
    {
        if (string.IsNullOrWhiteSpace(q))
            throw AppException.ValidationError("Query 'q' không được để trống");

        var result = new Dictionary<string, object>();

        if (type is "all" or "workout")
        {
            var exercises = await _db.Exercises.Where(e => EF.Functions.ILike(e.Name, $"%{q}%")).Take(10).ToListAsync();
            result["exercises"] = exercises.Select(e => new { e.Id, e.Name, e.MuscleGroup, DifficultyLevel = e.DifficultyLevel.ToString() });

            var routines = await _db.Routines.Where(r => EF.Functions.ILike(r.Name, $"%{q}%")).Take(10).ToListAsync();
            result["routines"] = routines.Select(r => new { r.Id, r.Name, Level = r.Level.ToString() });
        }

        if (type is "all" or "nutrition")
        {
            result["foods"] = await _db.FoodItems.Where(f => EF.Functions.ILike(f.Name, $"%{q}%"))
                .Take(10).Select(f => new { f.Id, f.Name, f.Calories }).ToListAsync();
            result["mealPlans"] = await _db.MealPlans.Where(m => EF.Functions.ILike(m.Name, $"%{q}%"))
                .Take(10).Select(m => new { m.Id, m.Name, m.Goal }).ToListAsync();
        }

        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }
}
