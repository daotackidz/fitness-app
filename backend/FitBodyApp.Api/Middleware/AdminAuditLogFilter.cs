using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitBodyApp.Api.Middleware;

public class AdminAuditLogFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> WriteMethods = new(StringComparer.OrdinalIgnoreCase) { "POST", "PATCH", "DELETE", "PUT" };
    private readonly FitBodyDbContext _db;

    public AdminAuditLogFilter(FitBodyDbContext db)
    {
        _db = db;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        var request = context.HttpContext.Request;
        if (!request.Path.StartsWithSegments("/api/admin") || !WriteMethods.Contains(request.Method))
            return;

        if (executed.Exception is not null || executed.Canceled) return;

        var userIdClaim = context.HttpContext.User.GetUserId();
        var controllerName = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerName ?? "Unknown";
        var targetId = context.ActionArguments.TryGetValue("id", out var idValue) && idValue is Guid guid ? guid : (Guid?)null;

        _db.AdminAuditLogs.Add(new AdminAuditLog
        {
            Id = Guid.NewGuid(),
            AdminUserId = userIdClaim,
            Action = $"{request.Method} {request.Path}",
            TargetType = controllerName,
            TargetId = targetId,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}
