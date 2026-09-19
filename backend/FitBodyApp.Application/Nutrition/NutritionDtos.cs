using System.Linq.Expressions;
using FitBodyApp.Domain.Entities;

namespace FitBodyApp.Application.Nutrition;

public record MealDto(Guid Id, string MealType, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG, string? ImageUrl);

public record MealPlanDto(Guid Id, string Name, string? Goal, string? Description, int? TotalCalories, bool IsCustom,
    Guid? CreatedByUserId, List<MealDto>? Meals);

public record CreateMealItem(string MealType, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG);

public record CreateMealPlanRequest(string Name, string? Goal, string? Description, List<CreateMealItem> Meals);

public record FoodItemDto(Guid Id, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG, string? ImageUrl, string? Category);

public static class MealMappings
{
    public static readonly Expression<Func<Meal, MealDto>> ToDto = m => new MealDto(
        m.Id, m.MealType.ToString(), m.Name, m.Calories, m.ProteinG, m.CarbsG, m.FatG,
        m.ImageFile != null ? m.ImageFile.Url : null);
}

public static class FoodItemMappings
{
    public static readonly Expression<Func<FoodItem, FoodItemDto>> ToDto = f => new FoodItemDto(
        f.Id, f.Name, f.Calories, f.ProteinG, f.CarbsG, f.FatG,
        f.ImageFile != null ? f.ImageFile.Url : null, f.Category);
}
