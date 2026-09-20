using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.MuscleGroup).HasColumnName("muscle_group").HasMaxLength(50);
        builder.Property(x => x.Equipment).HasColumnName("equipment").HasMaxLength(50);
        builder.Property(x => x.DifficultyLevel).HasColumnName("difficulty_level").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.VideoFileId).HasColumnName("video_file_id");
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");
        builder.Property(x => x.CaloriesEstimate).HasColumnName("calories_estimate");
        builder.Property(x => x.DurationMinutes).HasColumnName("duration_minutes");

        builder.HasIndex(x => x.MuscleGroup).HasDatabaseName("idx_exercises_muscle_group");
        builder.HasIndex(x => x.DifficultyLevel).HasDatabaseName("idx_exercises_difficulty");
        builder.HasIndex(x => x.Name).HasDatabaseName("idx_exercises_name_fts").HasMethod("gin").HasOperators("gin_trgm_ops");

        builder.HasOne(x => x.VideoFile).WithMany().HasForeignKey(x => x.VideoFileId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.ImageFile).WithMany().HasForeignKey(x => x.ImageFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class RoutineConfiguration : IEntityTypeConfiguration<Routine>
{
    public void Configure(EntityTypeBuilder<Routine> builder)
    {
        builder.ToTable("routines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Level).HasColumnName("level").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.DurationWeeks).HasColumnName("duration_weeks");
        builder.Property(x => x.IsCustom).HasColumnName("is_custom").HasDefaultValue(false);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");
        builder.Property(x => x.DurationMinutes).HasColumnName("duration_minutes");
        builder.Property(x => x.CaloriesEstimate).HasColumnName("calories_estimate");
        builder.Property(x => x.IsFeatured).HasColumnName("is_featured").HasDefaultValue(false);

        builder.HasIndex(x => x.Level).HasDatabaseName("idx_routines_level");
        builder.HasIndex(x => x.CreatedByUserId).HasDatabaseName("idx_routines_created_by");

        builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.ImageFile).WithMany().HasForeignKey(x => x.ImageFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class RoutineExerciseConfiguration : IEntityTypeConfiguration<RoutineExercise>
{
    public void Configure(EntityTypeBuilder<RoutineExercise> builder)
    {
        builder.ToTable("routine_exercises");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RoutineId).HasColumnName("routine_id");
        builder.Property(x => x.ExerciseId).HasColumnName("exercise_id");
        builder.Property(x => x.Sets).HasColumnName("sets");
        builder.Property(x => x.Reps).HasColumnName("reps");
        builder.Property(x => x.RestSeconds).HasColumnName("rest_seconds");
        builder.Property(x => x.OrderIndex).HasColumnName("order_index");
        builder.Property(x => x.RoundNumber).HasColumnName("round_number").HasDefaultValue(1);
        builder.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");

        builder.HasIndex(x => new { x.RoutineId, x.ExerciseId, x.OrderIndex }).IsUnique().HasDatabaseName("idx_routine_exercise");
        builder.HasIndex(x => x.RoutineId).HasDatabaseName("idx_routine_exercises_routine_id");

        builder.HasOne(x => x.Routine).WithMany(x => x.RoutineExercises).HasForeignKey(x => x.RoutineId);
        builder.HasOne(x => x.Exercise).WithMany(x => x.RoutineExercises).HasForeignKey(x => x.ExerciseId);
    }
}

public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
{
    public void Configure(EntityTypeBuilder<WorkoutLog> builder)
    {
        builder.ToTable("workout_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.RoutineId).HasColumnName("routine_id");
        builder.Property(x => x.ExerciseId).HasColumnName("exercise_id");
        builder.Property(x => x.LogDate).HasColumnName("log_date");
        builder.Property(x => x.DurationMinutes).HasColumnName("duration_minutes");
        builder.Property(x => x.SetsCompleted).HasColumnName("sets_completed");
        builder.Property(x => x.RepsCompleted).HasColumnName("reps_completed");
        builder.Property(x => x.WeightUsedKg).HasColumnName("weight_used_kg").HasColumnType("decimal(5,1)");
        builder.Property(x => x.CaloriesBurned).HasColumnName("calories_burned");

        builder.HasIndex(x => new { x.UserId, x.LogDate }).HasDatabaseName("idx_workout_logs_user_date");
        builder.HasIndex(x => x.RoutineId).HasDatabaseName("idx_workout_logs_routine_id");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

public class ProgressTrackingConfiguration : IEntityTypeConfiguration<ProgressTracking>
{
    public void Configure(EntityTypeBuilder<ProgressTracking> builder)
    {
        builder.ToTable("progress_tracking");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.RecordDate).HasColumnName("record_date");
        builder.Property(x => x.WeightKg).HasColumnName("weight_kg").HasColumnType("decimal(5,1)");
        builder.Property(x => x.BodyFatPercent).HasColumnName("body_fat_percent").HasColumnType("decimal(4,1)");
        builder.Property(x => x.MeasurementsJson).HasColumnName("measurements").HasColumnType("jsonb");
        builder.Property(x => x.PhotoImageId).HasColumnName("photo_image_id");

        builder.HasIndex(x => new { x.UserId, x.RecordDate }).HasDatabaseName("idx_progress_user_date");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.PhotoImage).WithMany().HasForeignKey(x => x.PhotoImageId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> builder)
    {
        builder.ToTable("recommendations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.ExerciseId).HasColumnName("exercise_id");
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.UserId).HasDatabaseName("idx_recommendations_user_id");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Exercise).WithMany().HasForeignKey(x => x.ExerciseId);
    }
}
