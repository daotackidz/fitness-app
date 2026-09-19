using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MuscleGroup { get; set; }
    public string? Equipment { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public Guid? VideoFileId { get; set; }
    public Guid? ImageFileId { get; set; }
    public int? CaloriesEstimate { get; set; }

    public VideoFile? VideoFile { get; set; }
    public ImageFile? ImageFile { get; set; }
    public ICollection<RoutineExercise> RoutineExercises { get; set; } = new List<RoutineExercise>();
}

public class Routine
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DifficultyLevel Level { get; set; }
    public string? Description { get; set; }
    public int? DurationWeeks { get; set; }
    public bool IsCustom { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }
    public ICollection<RoutineExercise> RoutineExercises { get; set; } = new List<RoutineExercise>();
}

public class RoutineExercise
{
    public Guid Id { get; set; }
    public Guid RoutineId { get; set; }
    public Guid ExerciseId { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int RestSeconds { get; set; }
    public int OrderIndex { get; set; }

    public Routine Routine { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}

public class WorkoutLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoutineId { get; set; }
    public Guid? ExerciseId { get; set; }
    public DateOnly LogDate { get; set; }
    public int? DurationMinutes { get; set; }
    public int? SetsCompleted { get; set; }
    public int? RepsCompleted { get; set; }
    public decimal? WeightUsedKg { get; set; }
    public int? CaloriesBurned { get; set; }

    public User User { get; set; } = null!;
}

public class ProgressTracking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly RecordDate { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? BodyFatPercent { get; set; }
    public string? MeasurementsJson { get; set; }
    public Guid? PhotoImageId { get; set; }

    public User User { get; set; } = null!;
    public ImageFile? PhotoImage { get; set; }
}

public class Recommendation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}
