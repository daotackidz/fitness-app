using FitBodyApp.Domain.Enums;

namespace FitBodyApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public FitnessGoal? FitnessGoal { get; set; }
    public ActivityLevel? ActivityLevel { get; set; }
    public Guid? AvatarImageId { get; set; }
    public bool IsProfileComplete { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ImageFile? AvatarImage { get; set; }
    public UserSetting? Settings { get; set; }
    public ICollection<AuthProvider> AuthProviders { get; set; } = new List<AuthProvider>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public class AuthProvider
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AuthProviderType Provider { get; set; }
    public string ProviderUid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }

    public User User { get; set; } = null!;
}

public class UserSetting
{
    public Guid UserId { get; set; }
    public bool NotificationEnabled { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;
    public bool DoNotDisturbEnabled { get; set; }
    public bool VibrateEnabled { get; set; } = true;
    public bool LockScreenEnabled { get; set; } = true;
    public bool RemindersEnabled { get; set; } = true;
    public TimeOnly? WorkoutReminderTime { get; set; }
    public string Language { get; set; } = "vi";

    public User User { get; set; } = null!;
}
