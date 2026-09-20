using FitBodyApp.Application.Common;
using FitBodyApp.Application.Users;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Infrastructure.Users;

public class UserService : IUserService
{
    private readonly FitBodyDbContext _db;
    private readonly IBlobStorageService _blobStorage;
    private readonly IMediaFileService _mediaFileService;

    public UserService(FitBodyDbContext db, IBlobStorageService blobStorage, IMediaFileService mediaFileService)
    {
        _db = db;
        _blobStorage = blobStorage;
        _mediaFileService = mediaFileService;
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await _db.Users.Include(u => u.AvatarImage).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw AppException.NotFound("Không tìm thấy người dùng");
        return ToProfileDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _db.Users.Include(u => u.AvatarImage).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw AppException.NotFound("Không tìm thấy người dùng");

        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.Nickname is not null) user.Nickname = request.Nickname;
        if (request.Gender is not null) user.Gender = Enum.Parse<Gender>(request.Gender, true);
        if (request.DateOfBirth is not null) user.DateOfBirth = request.DateOfBirth;
        if (request.HeightCm is not null) user.HeightCm = request.HeightCm;
        if (request.WeightKg is not null) user.WeightKg = request.WeightKg;
        if (request.FitnessGoal is not null) user.FitnessGoal = Enum.Parse<FitnessGoal>(request.FitnessGoal, true);
        if (request.ActivityLevel is not null) user.ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel, true);
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToProfileDto(user);
    }

    public async Task<UserProfileDto> CompleteOnboardingAsync(Guid userId, CompleteOnboardingRequest request)
    {
        var user = await _db.Users.Include(u => u.AvatarImage).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw AppException.NotFound("Không tìm thấy người dùng");

        user.FullName = request.FullName;
        user.Nickname = request.Nickname;
        if (request.Phone is not null) user.Phone = request.Phone;
        user.Gender = Enum.Parse<Gender>(request.Gender, true);
        user.DateOfBirth = request.DateOfBirth;
        user.HeightCm = request.HeightCm;
        user.WeightKg = request.WeightKg;
        user.FitnessGoal = Enum.Parse<FitnessGoal>(request.FitnessGoal, true);
        user.ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel, true);
        user.IsProfileComplete = true;
        user.UpdatedAt = DateTime.UtcNow;

        var now = DateTime.UtcNow;
        _db.Notifications.AddRange(
            new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(), UserId = user.Id, Type = NotificationType.System,
                Title = "Chào mừng bạn đến với FitBody!", Message = "Hồ sơ của bạn đã sẵn sàng, hãy bắt đầu hành trình luyện tập.",
                IsRead = false, CreatedAt = now
            },
            new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(), UserId = user.Id, Type = NotificationType.WorkoutReminder,
                Title = "Bài tập đầu tiên đang chờ bạn", Message = "Xem gợi ý bài tập phù hợp với mục tiêu của bạn ngay hôm nay.",
                IsRead = false, CreatedAt = now
            });

        await _db.SaveChangesAsync();
        return ToProfileDto(user);
    }

    public async Task<string> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName, string? contentType)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy người dùng");

        var uploadResult = await _blobStorage.UploadAsync(BlobContainers.Avatars, fileStream, fileName, contentType);
        var (imageId, url) = await _mediaFileService.SaveImageFileAsync(uploadResult, userId);

        user.AvatarImageId = imageId;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return url;
    }

    public async Task<UserSettingsDto> GetSettingsAsync(Guid userId)
    {
        var settings = await _db.UserSettings.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy cài đặt");
        return ToSettingsDto(settings);
    }

    public async Task<UserSettingsDto> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request)
    {
        var settings = await _db.UserSettings.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy cài đặt");

        if (request.NotificationEnabled is not null) settings.NotificationEnabled = request.NotificationEnabled.Value;
        if (request.SoundEnabled is not null) settings.SoundEnabled = request.SoundEnabled.Value;
        if (request.DoNotDisturbEnabled is not null) settings.DoNotDisturbEnabled = request.DoNotDisturbEnabled.Value;
        if (request.VibrateEnabled is not null) settings.VibrateEnabled = request.VibrateEnabled.Value;
        if (request.LockScreenEnabled is not null) settings.LockScreenEnabled = request.LockScreenEnabled.Value;
        if (request.RemindersEnabled is not null) settings.RemindersEnabled = request.RemindersEnabled.Value;
        if (request.WorkoutReminderTime is not null) settings.WorkoutReminderTime = request.WorkoutReminderTime;
        if (request.Language is not null) settings.Language = request.Language;

        await _db.SaveChangesAsync();
        return ToSettingsDto(settings);
    }

    private static UserSettingsDto ToSettingsDto(Domain.Entities.UserSetting s) => new(
        s.NotificationEnabled, s.SoundEnabled, s.DoNotDisturbEnabled, s.VibrateEnabled, s.LockScreenEnabled,
        s.RemindersEnabled, s.WorkoutReminderTime, s.Language);

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy người dùng");

        if (user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw AppException.ValidationError("Mật khẩu hiện tại không đúng");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Không tìm thấy người dùng");
        user.Status = UserStatus.Deleted;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private static UserProfileDto ToProfileDto(Domain.Entities.User user) => new(
        user.Id, user.FullName, user.Nickname, user.Email, user.Phone,
        user.Gender?.ToString(), user.DateOfBirth, user.HeightCm, user.WeightKg,
        user.FitnessGoal?.ToString(), user.ActivityLevel?.ToString(), user.AvatarImage?.Url,
        user.IsProfileComplete, user.Role.ToString(), user.Status.ToString());
}
