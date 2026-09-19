using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class MealPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public string? Description { get; set; }
    public int? TotalCalories { get; set; }
    public bool IsCustom { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }
    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}

public class Meal
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }
    public MealType MealType { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Calories { get; set; }
    public decimal? ProteinG { get; set; }
    public decimal? CarbsG { get; set; }
    public decimal? FatG { get; set; }
    public string? ImageUrl { get; set; }

    public MealPlan MealPlan { get; set; } = null!;
}

public class FoodItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Calories { get; set; }
    public decimal? ProteinG { get; set; }
    public decimal? CarbsG { get; set; }
    public decimal? FatG { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
}
