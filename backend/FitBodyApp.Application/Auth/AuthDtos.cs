namespace FitBodyApp.Application.Auth;

public record RegisterRequest(string FullName, string Email, string? Phone, string Password);
public record LoginRequest(string Email, string Password);
public record SocialLoginRequest(string Provider, string IdToken);
public record BiometricLoginRequest(string DeviceId, string BiometricToken);
public record RefreshTokenRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record VerifyResetCodeRequest(string Email, string Code);
public record ResetPasswordRequest(string Email, string Code, string NewPassword);

public record UserDto(Guid Id, string FullName, string Email, string? Phone, string Role, string Status, string? AvatarUrl, bool IsProfileComplete);

public record AuthResultDto(string AccessToken, string RefreshToken, int ExpiresInSeconds, UserDto User);
