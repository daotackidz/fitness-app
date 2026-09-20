using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Nutrition;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/meal-plans")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminMealPlansController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminMealPlansController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.MealPlans.OrderBy(m => m.Name)
            .Select(m => new MealPlanDto(m.Id, m.Name, m.Goal, m.Description, m.TotalCalories, m.IsCustom, m.CreatedByUserId, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<MealPlanDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertMealPlanRequest request)
    {
        var plan = new MealPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Goal = request.Goal,
            Description = request.Description,
            TotalCalories = request.TotalCalories,
            IsCustom = false
        };
        _db.MealPlans.Add(plan);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = plan.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertMealPlanRequest request)
    {
        var plan = await _db.MealPlans.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy meal plan");
        plan.Name = request.Name;
        plan.Goal = request.Goal;
        plan.Description = request.Description;
        plan.TotalCalories = request.TotalCalories;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var plan = await _db.MealPlans.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy meal plan");
        _db.MealPlans.Remove(plan);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
