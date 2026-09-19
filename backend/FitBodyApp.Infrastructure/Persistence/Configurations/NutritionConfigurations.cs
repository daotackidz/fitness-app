using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("meal_plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Goal).HasColumnName("goal").HasMaxLength(50);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.TotalCalories).HasColumnName("total_calories");
        builder.Property(x => x.IsCustom).HasColumnName("is_custom").HasDefaultValue(false);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");

        builder.HasIndex(x => x.Goal).HasDatabaseName("idx_meal_plans_goal");
        builder.HasIndex(x => x.CreatedByUserId).HasDatabaseName("idx_meal_plans_created_by");

        builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("meals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MealPlanId).HasColumnName("meal_plan_id");
        builder.Property(x => x.MealType).HasColumnName("meal_type").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Calories).HasColumnName("calories");
        builder.Property(x => x.ProteinG).HasColumnName("protein_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.CarbsG).HasColumnName("carbs_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.FatG).HasColumnName("fat_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(255);

        builder.HasIndex(x => new { x.MealPlanId, x.MealType }).HasDatabaseName("idx_meals_plan_type");

        builder.HasOne(x => x.MealPlan).WithMany(x => x.Meals).HasForeignKey(x => x.MealPlanId);
    }
}

public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.ToTable("food_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Calories).HasColumnName("calories");
        builder.Property(x => x.ProteinG).HasColumnName("protein_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.CarbsG).HasColumnName("carbs_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.FatG).HasColumnName("fat_g").HasColumnType("decimal(5,1)");
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(255);
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);

        builder.HasIndex(x => x.Name).HasDatabaseName("idx_food_items_name_fts").HasMethod("gin").HasOperators("gin_trgm_ops");
        builder.HasIndex(x => x.Category).HasDatabaseName("idx_food_items_category");
    }
}
