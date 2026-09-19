using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitBodyApp.Api.Middleware;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new InvalidOperationException("Khong tim thay claim sub trong token");
        return Guid.Parse(sub);
    }
}
