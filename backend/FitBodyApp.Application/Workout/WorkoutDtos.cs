namespace FitBodyApp.Application.Workout;

public record ExerciseDto(Guid Id, string Name, string? Description, string? MuscleGroup, string? Equipment,
    string DifficultyLevel, string? VideoUrl, string? ImageUrl, int? CaloriesEstimate);

public record RoutineExerciseDto(Guid ExerciseId, string ExerciseName, int Sets, int Reps, int RestSeconds, int OrderIndex);

public record RoutineDto(Guid Id, string Name, string Level, string? Description, int? DurationWeeks, bool IsCustom,
    Guid? CreatedByUserId, List<RoutineExerciseDto>? Exercises);

public record CreateRoutineRequest(string Name, string Level, string? Description, int? DurationWeeks,
    List<CreateRoutineExerciseItem> Exercises);

public record CreateRoutineExerciseItem(Guid ExerciseId, int Sets, int Reps, int RestSeconds, int OrderIndex);

public record UpdateRoutineRequest(string? Name, string? Description, int? DurationWeeks);

public record CreateWorkoutLogRequest(Guid? RoutineId, Guid? ExerciseId, DateOnly LogDate, int? DurationMinutes,
    int? SetsCompleted, int? RepsCompleted, decimal? WeightUsedKg, int? CaloriesBurned);

public record WorkoutLogDto(Guid Id, Guid? RoutineId, Guid? ExerciseId, DateOnly LogDate, int? DurationMinutes,
    int? SetsCompleted, int? RepsCompleted, decimal? WeightUsedKg, int? CaloriesBurned);

public record CreateProgressRequest(DateOnly RecordDate, decimal? WeightKg, decimal? BodyFatPercent,
    string? MeasurementsJson, string? PhotoUrl);

public record ProgressDto(Guid Id, DateOnly RecordDate, decimal? WeightKg, decimal? BodyFatPercent,
    string? MeasurementsJson, string? PhotoUrl);

public record RecommendationDto(Guid ExerciseId, string ExerciseName, string Reason);
