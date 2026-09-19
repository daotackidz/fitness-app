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

    public UserService(FitBodyDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");
        return ToProfileDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");

        if (request.FullName is not null) user.FullName = request.FullName;
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

    public async Task<string> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");

        var url = await _blobStorage.UploadAsync(BlobContainers.Avatars, fileStream, fileName, null);
        user.AvatarUrl = url;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return url;
    }

    public async Task<UserSettingsDto> GetSettingsAsync(Guid userId)
    {
        var settings = await _db.UserSettings.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay cai dat");
        return new UserSettingsDto(settings.NotificationEnabled, settings.WorkoutReminderTime, settings.Language);
    }

    public async Task<UserSettingsDto> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request)
    {
        var settings = await _db.UserSettings.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay cai dat");

        if (request.NotificationEnabled is not null) settings.NotificationEnabled = request.NotificationEnabled.Value;
        if (request.WorkoutReminderTime is not null) settings.WorkoutReminderTime = request.WorkoutReminderTime;
        if (request.Language is not null) settings.Language = request.Language;

        await _db.SaveChangesAsync();
        return new UserSettingsDto(settings.NotificationEnabled, settings.WorkoutReminderTime, settings.Language);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");

        if (user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw AppException.ValidationError("Mat khau hien tai khong dung");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay nguoi dung");
        user.Status = UserStatus.Deleted;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private static UserProfileDto ToProfileDto(Domain.Entities.User user) => new(
        user.Id, user.FullName, user.Email, user.Phone,
        user.Gender?.ToString(), user.DateOfBirth, user.HeightCm, user.WeightKg,
        user.FitnessGoal?.ToString(), user.ActivityLevel?.ToString(), user.AvatarUrl,
        user.Role.ToString(), user.Status.ToString());
}
