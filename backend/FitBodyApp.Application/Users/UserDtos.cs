namespace FitBodyApp.Application.Users;

public record UserProfileDto(
    Guid Id, string FullName, string Email, string? Phone,
    string? Gender, DateOnly? DateOfBirth, decimal? HeightCm, decimal? WeightKg,
    string? FitnessGoal, string? ActivityLevel, string? AvatarUrl, string Role, string Status);

public record UpdateProfileRequest(
    string? FullName, string? Gender, DateOnly? DateOfBirth,
    decimal? HeightCm, decimal? WeightKg, string? FitnessGoal, string? ActivityLevel);

public record UserSettingsDto(bool NotificationEnabled, TimeOnly? WorkoutReminderTime, string Language);

public record UpdateSettingsRequest(bool? NotificationEnabled, TimeOnly? WorkoutReminderTime, string? Language);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
