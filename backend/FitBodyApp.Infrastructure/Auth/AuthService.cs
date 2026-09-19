using System.Security.Cryptography;
using System.Text;
using FitBodyApp.Application.Auth;
using FitBodyApp.Application.Common;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FitBodyApp.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly FitBodyDbContext _db;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IMemoryCache _cache;

    public AuthService(FitBodyDbContext db, IJwtTokenGenerator tokenGenerator, IMemoryCache cache)
    {
        _db = db;
        _tokenGenerator = tokenGenerator;
        _cache = cache;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterRequest request)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            throw AppException.Conflict("Email da duoc su dung");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active,
            Role = UserRole.User,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Users.Add(user);
        _db.UserSettings.Add(new UserSetting { UserId = user.Id });
        await _db.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw AppException.Unauthorized("Email hoac mat khau khong dung");

        if (user.Status != UserStatus.Active)
            throw AppException.Forbidden("Tai khoan da bi khoa hoac xoa");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResultDto> SocialLoginAsync(SocialLoginRequest request)
    {
        if (!Enum.TryParse<AuthProviderType>(request.Provider, true, out var providerType) ||
            providerType is not (AuthProviderType.Google or AuthProviderType.Facebook))
            throw AppException.ValidationError("Provider khong hop le");

        // TODO: xac thuc id_token thuc su qua Google/Facebook SDK khi co credential; hien tam tin tuong id_token la provider_uid da xac thuc phia client
        var providerUid = request.IdToken;
        var authProvider = await _db.AuthProviders
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Provider == providerType && x.ProviderUid == providerUid);

        if (authProvider is null)
            throw AppException.Unauthorized("Tai khoan social chua duoc lien ket, vui long dang ky truoc");

        if (authProvider.User.Status != UserStatus.Active)
            throw AppException.Forbidden("Tai khoan da bi khoa hoac xoa");

        return await IssueTokensAsync(authProvider.User);
    }

    public async Task<AuthResultDto> BiometricLoginAsync(BiometricLoginRequest request)
    {
        var authProvider = await _db.AuthProviders
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Provider == AuthProviderType.Fingerprint && x.ProviderUid == request.DeviceId);

        if (authProvider is null)
            throw AppException.Unauthorized("Thiet bi chua duoc dang ky dang nhap sinh trac hoc");

        if (authProvider.User.Status != UserStatus.Active)
            throw AppException.Forbidden("Tai khoan da bi khoa hoac xoa");

        return await IssueTokensAsync(authProvider.User);
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var stored = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.Revoked);

        if (stored is null || stored.ExpiresAt < DateTime.UtcNow)
            throw AppException.Unauthorized("Refresh token khong hop le hoac da het han");

        stored.Revoked = true;
        return await IssueTokensAsync(stored.User);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        if (stored is not null)
        {
            stored.Revoked = true;
            await _db.SaveChangesAsync();
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null) return;

        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _cache.Set($"password-reset:{resetToken}", user.Id, TimeSpan.FromMinutes(15));

        // TODO: tich hop email/SMS that; hien in ra console de test luong dev
        Console.WriteLine($"[FitBodyApp] Reset password token cho {user.Email}: {resetToken} (het han sau 15 phut)");
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (!_cache.TryGetValue($"password-reset:{request.Token}", out Guid userId))
            throw AppException.ValidationError("Token khong hop le hoac da het han");

        var user = await _db.Users.FindAsync(userId) ?? throw AppException.NotFound("Khong tim thay tai khoan");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        _cache.Remove($"password-reset:{request.Token}");
        await _db.SaveChangesAsync();
    }

    private async Task<AuthResultDto> IssueTokensAsync(User user)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenGenerator.RefreshTokenDays),
            Revoked = false
        });
        await _db.SaveChangesAsync();

        var avatarUrl = user.AvatarImageId.HasValue
            ? await _db.ImageFiles.Where(i => i.Id == user.AvatarImageId).Select(i => i.Url).FirstOrDefaultAsync()
            : null;

        var userDto = new UserDto(user.Id, user.FullName, user.Email, user.Phone, user.Role.ToString(), user.Status.ToString(), avatarUrl);
        return new AuthResultDto(accessToken, refreshToken, _tokenGenerator.AccessTokenMinutes * 60, userDto);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
