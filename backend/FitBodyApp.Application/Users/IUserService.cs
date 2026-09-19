namespace FitBodyApp.Application.Users;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(Guid userId);
    Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<string> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName);
    Task<UserSettingsDto> GetSettingsAsync(Guid userId);
    Task<UserSettingsDto> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task DeleteAccountAsync(Guid userId);
}
