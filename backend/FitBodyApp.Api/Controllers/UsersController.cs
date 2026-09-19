using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var profile = await _userService.GetProfileAsync(User.GetUserId());
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        var profile = await _userService.UpdateProfileAsync(User.GetUserId(), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    [HttpPost("me/avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var url = await _userService.UpdateAvatarAsync(User.GetUserId(), stream, file.FileName);
        return Ok(ApiResponse<object>.Ok(new { avatarUrl = url }));
    }

    [HttpGet("me/settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _userService.GetSettingsAsync(User.GetUserId());
        return Ok(ApiResponse<UserSettingsDto>.Ok(settings));
    }

    [HttpPatch("me/settings")]
    public async Task<IActionResult> UpdateSettings(UpdateSettingsRequest request)
    {
        var settings = await _userService.UpdateSettingsAsync(User.GetUserId(), request);
        return Ok(ApiResponse<UserSettingsDto>.Ok(settings));
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        await _userService.ChangePasswordAsync(User.GetUserId(), request);
        return Ok(ApiResponse<object>.Ok(new { message = "Doi mat khau thanh cong" }));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        await _userService.DeleteAccountAsync(User.GetUserId());
        return Ok(ApiResponse<object>.Ok(new { message = "Da xoa tai khoan" }));
    }
}
