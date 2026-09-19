namespace FitBodyApp.Application.Nutrition;

public record MealDto(Guid Id, string MealType, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG, string? ImageUrl);

public record MealPlanDto(Guid Id, string Name, string? Goal, string? Description, int? TotalCalories, bool IsCustom,
    Guid? CreatedByUserId, List<MealDto>? Meals);

public record CreateMealItem(string MealType, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG);

public record CreateMealPlanRequest(string Name, string? Goal, string? Description, List<CreateMealItem> Meals);

public record FoodItemDto(Guid Id, string Name, int? Calories, decimal? ProteinG, decimal? CarbsG, decimal? FatG, string? ImageUrl, string? Category);
