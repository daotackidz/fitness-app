using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public FavoritesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFavoriteRequest request)
    {
        if (!Enum.TryParse<FavoritableType>(request.FavoritableType, true, out var type))
            throw AppException.ValidationError("favoritable_type khong hop le");

        var exists = type == FavoritableType.Article
            ? await _db.Articles.AnyAsync(a => a.Id == request.FavoritableId)
            : await _db.Videos.AnyAsync(v => v.Id == request.FavoritableId);
        if (!exists)
            throw AppException.NotFound("Khong tim thay noi dung de yeu thich");

        var userId = User.GetUserId();
        var already = await _db.Favorites.AnyAsync(f => f.UserId == userId && f.FavoritableType == type && f.FavoritableId == request.FavoritableId);
        if (already)
            throw AppException.Conflict("Da yeu thich noi dung nay roi");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FavoritableType = type,
            FavoritableId = request.FavoritableId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Favorites.Add(favorite);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = favorite.Id }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId)
            ?? throw AppException.NotFound("Khong tim thay muc yeu thich");

        _db.Favorites.Remove(favorite);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = _db.Favorites.Where(f => f.UserId == userId);

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<FavoritableType>(type, true, out var favType))
            query = query.Where(f => f.FavoritableType == favType);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavoriteDto(f.Id, f.FavoritableType.ToString(), f.FavoritableId, f.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<FavoriteDto>>.Ok(items, meta));
    }
}
