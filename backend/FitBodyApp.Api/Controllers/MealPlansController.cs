using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Nutrition;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/meal-plans")]
[Authorize]
public class MealPlansController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public MealPlansController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? goal, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.MealPlans.AsQueryable();
        if (!string.IsNullOrWhiteSpace(goal)) query = query.Where(m => m.Goal == goal);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(m => m.Name)
            .Select(m => new MealPlanDto(m.Id, m.Name, m.Goal, m.Description, m.TotalCalories, m.IsCustom, m.CreatedByUserId, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<MealPlanDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var plan = await _db.MealPlans.Include(m => m.Meals).FirstOrDefaultAsync(m => m.Id == id)
            ?? throw AppException.NotFound("Khong tim thay meal plan");

        var meals = plan.Meals.Select(m => new MealDto(m.Id, m.MealType.ToString(), m.Name, m.Calories, m.ProteinG, m.CarbsG, m.FatG, m.ImageUrl)).ToList();
        var dto = new MealPlanDto(plan.Id, plan.Name, plan.Goal, plan.Description, plan.TotalCalories, plan.IsCustom, plan.CreatedByUserId, meals);
        return Ok(ApiResponse<MealPlanDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMealPlanRequest request)
    {
        var userId = User.GetUserId();
        var plan = new MealPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Goal = request.Goal,
            Description = request.Description,
            IsCustom = true,
            CreatedByUserId = userId,
            TotalCalories = request.Meals.Sum(m => m.Calories ?? 0)
        };
        _db.MealPlans.Add(plan);

        foreach (var item in request.Meals)
        {
            if (!Enum.TryParse<MealType>(item.MealType, true, out var mealType))
                throw AppException.ValidationError($"meal_type khong hop le: {item.MealType}");

            _db.Meals.Add(new Meal
            {
                Id = Guid.NewGuid(),
                MealPlanId = plan.Id,
                MealType = mealType,
                Name = item.Name,
                Calories = item.Calories,
                ProteinG = item.ProteinG,
                CarbsG = item.CarbsG,
                FatG = item.FatG
            });
        }
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetail), new { id = plan.Id }, ApiResponse<object>.Ok(new { id = plan.Id }));
    }
}
