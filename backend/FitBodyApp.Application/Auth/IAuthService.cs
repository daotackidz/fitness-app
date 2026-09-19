namespace FitBodyApp.Application.Auth;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterRequest request);
    Task<AuthResultDto> LoginAsync(LoginRequest request);
    Task<AuthResultDto> SocialLoginAsync(SocialLoginRequest request);
    Task<AuthResultDto> BiometricLoginAsync(BiometricLoginRequest request);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(string refreshToken);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    int AccessTokenMinutes { get; }
    int RefreshTokenDays { get; }
}
