using System.Linq.Expressions;
using FitBodyApp.Domain.Entities;

namespace FitBodyApp.Application.Workout;

public record ExerciseDto(Guid Id, string Name, string? Description, string? MuscleGroup, string? Equipment,
    string DifficultyLevel, string? VideoUrl, string? ImageUrl, int? CaloriesEstimate, int? DurationMinutes,
    Guid? VideoFileId, Guid? ImageFileId);

public static class ExerciseMappings
{
    public static readonly Expression<Func<Exercise, ExerciseDto>> ToDto = e => new ExerciseDto(
        e.Id, e.Name, e.Description, e.MuscleGroup, e.Equipment, e.DifficultyLevel.ToString(),
        e.VideoFile != null ? e.VideoFile.Url : null,
        e.ImageFile != null ? e.ImageFile.Url : null,
        e.CaloriesEstimate, e.DurationMinutes, e.VideoFileId, e.ImageFileId);
}

public record RoutineExerciseDto(Guid ExerciseId, string ExerciseName, int Sets, int Reps, int RestSeconds, int OrderIndex,
    int RoundNumber, int? DurationSeconds, string? ImageUrl, string? Description, int? CaloriesEstimate, string DifficultyLevel);

public record RoutineDto(Guid Id, string Name, string Level, string? Description, int? DurationWeeks, bool IsCustom,
    Guid? CreatedByUserId, string? ImageUrl, int? DurationMinutes, int? CaloriesEstimate, int ExerciseCount,
    bool IsFeatured, bool IsFavorited, List<RoutineExerciseDto>? Exercises);

public record CreateRoutineRequest(string Name, string Level, string? Description, int? DurationWeeks,
    List<CreateRoutineExerciseItem> Exercises);

public record CreateRoutineExerciseItem(Guid ExerciseId, int Sets, int Reps, int RestSeconds, int OrderIndex,
    int RoundNumber, int? DurationSeconds);

public record UpdateRoutineRequest(string? Name, string? Description, int? DurationWeeks);

public record CreateWorkoutLogRequest(Guid? RoutineId, Guid? ExerciseId, DateOnly LogDate, int? DurationMinutes,
    int? SetsCompleted, int? RepsCompleted, decimal? WeightUsedKg, int? CaloriesBurned);

public record WorkoutLogDto(Guid Id, Guid? RoutineId, Guid? ExerciseId, DateOnly LogDate, int? DurationMinutes,
    int? SetsCompleted, int? RepsCompleted, decimal? WeightUsedKg, int? CaloriesBurned);

public record CreateProgressRequest(DateOnly RecordDate, decimal? WeightKg, decimal? BodyFatPercent,
    string? MeasurementsJson, Guid? PhotoImageId);

public record ProgressDto(Guid Id, DateOnly RecordDate, decimal? WeightKg, decimal? BodyFatPercent,
    string? MeasurementsJson, string? PhotoUrl, Guid? PhotoImageId);

public static class ProgressMappings
{
    public static readonly Expression<Func<ProgressTracking, ProgressDto>> ToDto = p => new ProgressDto(
        p.Id, p.RecordDate, p.WeightKg, p.BodyFatPercent, p.MeasurementsJson,
        p.PhotoImage != null ? p.PhotoImage.Url : null, p.PhotoImageId);
}

public record RecommendationDto(Guid ExerciseId, string ExerciseName, string Reason);
