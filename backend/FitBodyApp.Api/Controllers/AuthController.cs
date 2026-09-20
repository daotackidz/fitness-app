using FitBodyApp.Application.Auth;
using FitBodyApp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("social-login")]
    public async Task<IActionResult> SocialLogin(SocialLoginRequest request)
    {
        var result = await _authService.SocialLoginAsync(request);
        return Ok(ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("biometric-login")]
    public async Task<IActionResult> BiometricLogin(BiometricLoginRequest request)
    {
        var result = await _authService.BiometricLoginAsync(request);
        return Ok(ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Ok(ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request);
        return Ok(ApiResponse<object>.Ok(new { message = "Nếu email tồn tại, mã xác nhận đã được gửi" }));
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest request)
    {
        await _authService.VerifyResetCodeAsync(request);
        return Ok(ApiResponse<object>.Ok(new { message = "Mã xác nhận hợp lệ" }));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request);
        return Ok(ApiResponse<object>.Ok(new { message = "Đặt lại mật khẩu thành công" }));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return Ok(ApiResponse<object>.Ok(new { message = "Đã đăng xuất" }));
    }
}
