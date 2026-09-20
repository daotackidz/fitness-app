namespace FitBodyApp.Application.Users;

public record UserProfileDto(
    Guid Id, string FullName, string? Nickname, string Email, string? Phone,
    string? Gender, DateOnly? DateOfBirth, decimal? HeightCm, decimal? WeightKg,
    string? FitnessGoal, string? ActivityLevel, string? AvatarUrl, bool IsProfileComplete,
    string Role, string Status);

public record UpdateProfileRequest(
    string? FullName, string? Nickname, string? Gender, DateOnly? DateOfBirth,
    decimal? HeightCm, decimal? WeightKg, string? FitnessGoal, string? ActivityLevel);

public record CompleteOnboardingRequest(
    string FullName, string? Nickname, string? Phone,
    string Gender, DateOnly DateOfBirth, decimal HeightCm, decimal WeightKg,
    string FitnessGoal, string ActivityLevel);

public record UserSettingsDto(bool NotificationEnabled, bool SoundEnabled, bool DoNotDisturbEnabled,
    bool VibrateEnabled, bool LockScreenEnabled, bool RemindersEnabled, TimeOnly? WorkoutReminderTime, string Language);

public record UpdateSettingsRequest(bool? NotificationEnabled, bool? SoundEnabled, bool? DoNotDisturbEnabled,
    bool? VibrateEnabled, bool? LockScreenEnabled, bool? RemindersEnabled, TimeOnly? WorkoutReminderTime, string? Language);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
