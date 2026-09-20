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
        Console.WriteLine("Đã seed tài khoản super_admin mặc định:");
        Console.WriteLine($"  Email:    {admin.Email}");
        Console.WriteLine($"  Password: {password}");
        Console.WriteLine("Đổi mật khẩu ngay sau khi đăng nhập lần đầu.");
        Console.WriteLine("==============================================");
    }

    private static async Task SeedContentAsync(FitBodyDbContext db)
    {
        if (!await db.Exercises.AnyAsync())
        {
            var exercises = new[]
            {
                new Exercise { Id = Guid.NewGuid(), Name = "Push Up", MuscleGroup = "Chest", Equipment = "None", DifficultyLevel = DifficultyLevel.Beginner, CaloriesEstimate = 8, DurationMinutes = 10 },
                new Exercise { Id = Guid.NewGuid(), Name = "Squat", MuscleGroup = "Legs", Equipment = "None", DifficultyLevel = DifficultyLevel.Beginner, CaloriesEstimate = 9, DurationMinutes = 12 },
                new Exercise { Id = Guid.NewGuid(), Name = "Deadlift", MuscleGroup = "Back", Equipment = "Barbell", DifficultyLevel = DifficultyLevel.Intermediate, CaloriesEstimate = 12, DurationMinutes = 15 },
                new Exercise { Id = Guid.NewGuid(), Name = "Pull Up", MuscleGroup = "Back", Equipment = "Pull-up Bar", DifficultyLevel = DifficultyLevel.Intermediate, CaloriesEstimate = 10, DurationMinutes = 10 },
                new Exercise { Id = Guid.NewGuid(), Name = "Muscle Up", MuscleGroup = "Full Body", Equipment = "Pull-up Bar", DifficultyLevel = DifficultyLevel.Advanced, CaloriesEstimate = 15, DurationMinutes = 15 }
            };
            db.Exercises.AddRange(exercises);
            await db.SaveChangesAsync();

            var routines = new[]
            {
                new Routine { Id = Guid.NewGuid(), Name = "Beginner Full Body", Level = DifficultyLevel.Beginner, DurationWeeks = 4, IsCustom = false, DurationMinutes = 45, CaloriesEstimate = 450, IsFeatured = true },
                new Routine { Id = Guid.NewGuid(), Name = "Intermediate Strength", Level = DifficultyLevel.Intermediate, DurationWeeks = 6, IsCustom = false, DurationMinutes = 50, CaloriesEstimate = 520 },
                new Routine { Id = Guid.NewGuid(), Name = "Advanced Calisthenics", Level = DifficultyLevel.Advanced, DurationWeeks = 8, IsCustom = false, DurationMinutes = 35, CaloriesEstimate = 380 }
            };
            db.Routines.AddRange(routines);
            await db.SaveChangesAsync();

            db.RoutineExercises.AddRange(
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[0].Id, ExerciseId = exercises[0].Id, Sets = 3, Reps = 12, RestSeconds = 60, OrderIndex = 1, RoundNumber = 1, DurationSeconds = 30 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[0].Id, ExerciseId = exercises[1].Id, Sets = 3, Reps = 15, RestSeconds = 60, OrderIndex = 2, RoundNumber = 1, DurationSeconds = 30 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[0].Id, ExerciseId = exercises[3].Id, Sets = 3, Reps = 10, RestSeconds = 45, OrderIndex = 3, RoundNumber = 2, DurationSeconds = 20 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[1].Id, ExerciseId = exercises[2].Id, Sets = 4, Reps = 8, RestSeconds = 90, OrderIndex = 1, RoundNumber = 1, DurationSeconds = 40 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[1].Id, ExerciseId = exercises[3].Id, Sets = 4, Reps = 8, RestSeconds = 90, OrderIndex = 2, RoundNumber = 1, DurationSeconds = 40 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[1].Id, ExerciseId = exercises[4].Id, Sets = 3, Reps = 6, RestSeconds = 60, OrderIndex = 3, RoundNumber = 2, DurationSeconds = 30 },
                new RoutineExercise { Id = Guid.NewGuid(), RoutineId = routines[2].Id, ExerciseId = exercises[4].Id, Sets = 5, Reps = 5, RestSeconds = 120, OrderIndex = 1, RoundNumber = 1, DurationSeconds = 45 }
            );
            await db.SaveChangesAsync();
        }

        var routinesMissingStats = await db.Routines.Where(r => r.DurationMinutes == null).ToListAsync();
        if (routinesMissingStats.Count > 0)
        {
            foreach (var r in routinesMissingStats) { r.DurationMinutes = 40; r.CaloriesEstimate = 400; }
            await db.SaveChangesAsync();
        }

        var beginnerFeatured = await db.Routines.Where(r => r.Level == DifficultyLevel.Beginner)
            .OrderBy(r => r.Name).FirstOrDefaultAsync();
        if (beginnerFeatured is not null)
        {
            var wrongFeatured = await db.Routines.Where(r => r.IsFeatured && r.Id != beginnerFeatured.Id).ToListAsync();
            var changed = wrongFeatured.Count > 0 || !beginnerFeatured.IsFeatured;
            foreach (var r in wrongFeatured) r.IsFeatured = false;
            beginnerFeatured.IsFeatured = true;
            if (changed) await db.SaveChangesAsync();
        }

        var routinesMissingRound2 = await db.Routines
            .Where(r => !r.RoutineExercises.Any(re => re.RoundNumber == 2))
            .Include(r => r.RoutineExercises)
            .ToListAsync();
        if (routinesMissingRound2.Count > 0)
        {
            var anyExercise = await db.Exercises.OrderBy(e => e.Name).FirstOrDefaultAsync();
            if (anyExercise is not null)
            {
                foreach (var r in routinesMissingRound2)
                {
                    var nextOrder = r.RoutineExercises.Count == 0 ? 1 : r.RoutineExercises.Max(re => re.OrderIndex) + 1;
                    db.RoutineExercises.Add(new RoutineExercise
                    {
                        Id = Guid.NewGuid(), RoutineId = r.Id, ExerciseId = anyExercise.Id,
                        Sets = 3, Reps = 10, RestSeconds = 30, OrderIndex = nextOrder, RoundNumber = 2, DurationSeconds = 20
                    });
                }
                await db.SaveChangesAsync();
            }
        }

        if (!await db.MealPlans.AnyAsync())
        {
            var mealPlan = new MealPlan { Id = Guid.NewGuid(), Name = "Chế độ ăn cân bằng", Goal = "lose_weight", TotalCalories = 1800, IsCustom = false };
            db.MealPlans.Add(mealPlan);
            await db.SaveChangesAsync();

            db.Meals.AddRange(
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Breakfast, Name = "Yến mạch + chuối", Calories = 350, ProteinG = 12, CarbsG = 60, FatG = 6 },
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Lunch, Name = "Ức gà + cơm gạo lứt", Calories = 550, ProteinG = 45, CarbsG = 55, FatG = 12 },
                new Meal { Id = Guid.NewGuid(), MealPlanId = mealPlan.Id, MealType = MealType.Dinner, Name = "Cá hồi + rau củ", Calories = 500, ProteinG = 38, CarbsG = 30, FatG = 20 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Articles.AnyAsync())
        {
            db.Articles.AddRange(
                new Article { Id = Guid.NewGuid(), Title = "5 loi ich cua tap luyen deu dan", Category = "health", Author = "FitBody Team", PublishedAt = DateTime.UtcNow },
                new Article { Id = Guid.NewGuid(), Title = "Dinh dưỡng cho người mới bắt đầu tập gym", Category = "nutrition", Author = "FitBody Team", PublishedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Challenges.AnyAsync())
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            db.Challenges.Add(new Challenge
            {
                Id = Guid.NewGuid(),
                Name = "Plank With Hip Twist",
                Description = "Hoàn thành plank kết hợp xoay hông mỗi ngày trong tuần này.",
                Type = ChallengeType.Weekly,
                StartDate = today.AddDays(-(int)today.DayOfWeek),
                EndDate = today.AddDays(6),
                GoalMetric = "7 ngày liên tiếp",
                Reward = "Huy hiệu Kiên Trì"
            });
            await db.SaveChangesAsync();
        }

        var exercisesMissingDuration = await db.Exercises.Where(e => e.DurationMinutes == null).ToListAsync();
        if (exercisesMissingDuration.Count > 0)
        {
            foreach (var e in exercisesMissingDuration) e.DurationMinutes = 12;
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

            db.Videos.Add(new Video
            {
                Id = Guid.NewGuid(), Title = "Hướng dẫn Push Up đúng kỹ thuật", VideoFileId = seedVideoFile.Id,
                DurationSeconds = 180, Category = "workout", CaloriesEstimate = 60, ExerciseCount = 1
            });
            await db.SaveChangesAsync();
        }

        var videosMissingStats = await db.Videos.Where(v => v.CaloriesEstimate == null).ToListAsync();
        if (videosMissingStats.Count > 0)
        {
            foreach (var v in videosMissingStats) v.CaloriesEstimate = 60;
            await db.SaveChangesAsync();
        }

        if (!await db.Faqs.AnyAsync())
        {
            db.Faqs.AddRange(
                new Faq { Id = Guid.NewGuid(), Question = "Làm sao để đổi mật khẩu?", Answer = "Vào Cài đặt > Đổi mật khẩu.", Category = "account" },
                new Faq { Id = Guid.NewGuid(), Question = "Ứng dụng có hỗ trợ đồng bộ thiết bị đeo tay?", Answer = "Hiện tại chưa hỗ trợ, sẽ cập nhật trong tương lai.", Category = "general" },
                new Faq { Id = Guid.NewGuid(), Question = "Làm sao để liên hệ hỗ trợ khách hàng?", Answer = "Vào mục Hỗ trợ > Liên hệ để chat trực tiếp.", Category = "services" }
            );
            await db.SaveChangesAsync();
        }
    }
}
