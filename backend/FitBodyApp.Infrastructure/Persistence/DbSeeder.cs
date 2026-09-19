using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(FitBodyDbContext db)
    {
        await SeedSuperAdminAsync(db);
        await SeedContentAsync(db);
    }

    private static async Task SeedSuperAdminAsync(FitBodyDbContext db)
    {
        var exists = await db.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin);
        if (exists) return;

        var password = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(9));
        var now = DateTime.UtcNow;
        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Super Admin",
            Email = "admin@fitbody.dev",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Status = UserStatus.Active,
            Role = UserRole.SuperAdmin,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.Users.Add(admin);
        db.UserSettings.Add(new UserSetting { UserId = admin.Id });
        await db.SaveChangesAsync();

        Console.WriteLine("==============================================");
        Console.WriteLine("Da seed tai khoan super_admin mac dinh:");
        Console.WriteLine($"  Email:    {admin.Email}");
        Console.WriteLine($"  Password: {password}");
        Console.WriteLine("Doi mat khau ngay sau khi dang nhap lan dau.");
        Console.WriteLine("==============================================");
    }

    private static async Task SeedContentAsync(FitBodyDbContext db)
    {
        if (!await db.Exercises.AnyAsync())
        {
            var exercises = new[]
            {
                new Exercise { Id = Guid.NewGuid(), Name = "Push Up", MuscleGroup = "Chest", Equipment = "None", DifficultyLevel = DifficultyLevel.Beginner, CaloriesEstimate = 8 },
                new Exercise { Id = Guid.NewGuid(), Name = "Squat", MuscleGroup = "Legs", Equipment = "None", DifficultyLevel = DifficultyLevel.Beginner, CaloriesEstimate = 9 },
                new Exercise { Id = Guid.NewGuid(), Name = "Deadlift", MuscleGroup = "Back", Equipment = "Barbell", DifficultyLevel = DifficultyLevel.Intermediate, CaloriesEstimate = 12 },
                new Exercise { Id = Guid.NewGuid(), Name = "Pull Up", MuscleGroup = "Back", Equipment = "Pull-up Bar", DifficultyLevel = DifficultyLevel.Intermediate, CaloriesEstimate = 10 },
                new Exercise { Id = Guid.NewGuid(), Name = "Muscle Up", MuscleGroup = "Full Body", Equipment = "Pull-up Bar", DifficultyLevel = DifficultyLevel.Advanced, CaloriesEstimate = 15 }
            };
            db.Exercises.AddRange(exercises);
            await db.SaveChangesAsync();

            var routines = new[]
            {
                new Routine { Id = Guid.NewGuid(), Name = "Beginner Full Body", Level = DifficultyLevel.Beginner, DurationWeeks = 4, IsCustom = false },
                new Routine { Id = Guid.NewGuid(), Name = "Intermediate Strength", Level = DifficultyLevel.Intermediate, DurationWeeks = 6, IsCustom = false },
                new Routine { Id = Guid.NewGuid(), Name = "Advanced Calisthenics", Level = DifficultyLevel.Advanced, DurationWeeks = 8, IsCustom = false }
            };
            db.Routines.AddRange(routines);
            await db.SaveChangesAsync();

            db.RoutineExercises.AddRange(
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[0].Id, ExerciseId = exercises[0].Id, Sets = 3, Reps = 12, RestSeconds = 60, OrderIndex = 1 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[0].Id, ExerciseId = exercises[1].Id, Sets = 3, Reps = 15, RestSeconds = 60, OrderIndex = 2 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[1].Id, ExerciseId = exercises[2].Id, Sets = 4, Reps = 8, RestSeconds = 90, OrderIndex = 1 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[1].Id, ExerciseId = exercises[3].Id, Sets = 4, Reps = 8, RestSeconds = 90, OrderIndex = 2 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[2].Id, ExerciseId = exercises[4].Id, Sets = 5, Reps = 5, RestSeconds = 120, OrderIndex = 1 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.MealPlans.AnyAsync())
        {
            var mealPlan = new MealPlan { Id = Guid.NewGuid(), Name = "Loi bat can bang", Goal = "lose_weight", TotalCalories = 1800, IsCustom = false };
            db.MealPlans.Add(mealPlan);
            await db.SaveChangesAsync();

            db.Meals.AddRange(
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Breakfast, Name = "Yen mach + chuoi", Calories = 350, ProteinG = 12, CarbsG = 60, FatG = 6 },
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Lunch, Name = "Uc ga + com gao lut", Calories = 550, ProteinG = 45, CarbsG = 55, FatG = 12 },
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Dinner, Name = "Ca hoi + rau cu", Calories = 500, ProteinG = 38, CarbsG = 30, FatG = 20 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Articles.AnyAsync())
        {
            db.Articles.AddRange(
                new Article { Id = Guid.NewGuid(), Title = "5 loi ich cua tap luyen deu dan", Category = "health", Author = "FitBody Team", PublishedAt = DateTime.UtcNow },
                new Article { Id = Guid.NewGuid(), Title = "Dinh duong cho nguoi moi bat dau tap gym", Category = "nutrition", Author = "FitBody Team", PublishedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Videos.AnyAsync())
        {
            var seedVideoFile = new VideoFile
            {
                Id = Guid.NewGuid(),
                Url = "https://example.com/video/push-up.mp4",
                BlobName = "seed-push-up",
                Container = "external",
                FileName = "push-up.mp4",
                ContentType = "video/mp4",
                SizeBytes = 0,
                CreatedAt = DateTime.UtcNow
            };
            db.VideoFiles.Add(seedVideoFile);
            await db.SaveChangesAsync();

            db.Videos.Add(new Video { Id = Guid.NewGuid(), Title = "Huong dan Push Up dung ky thuat", VideoFileId = seedVideoFile.Id, DurationSeconds = 180, Category = "workout" });
            await db.SaveChangesAsync();
        }

        if (!await db.Faqs.AnyAsync())
        {
            db.Faqs.AddRange(
                new Faq { Id = Guid.NewGuid(), Question = "Lam sao de doi mat khau?", Answer = "Vao Cai dat > Doi mat khau.", Category = "account" },
                new Faq { Id = Guid.NewGuid(), Question = "Ung dung co ho tro dong bo thiet bi deo tay?", Answer = "Hien tai chua ho tro, se cap nhat trong tuong lai.", Category = "general" }
            );
            await db.SaveChangesAsync();
        }
    }
}
