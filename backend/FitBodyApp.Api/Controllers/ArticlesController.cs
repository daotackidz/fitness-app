using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/articles")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public ArticlesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Articles.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(a => a.Category == category);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(a => a.PublishedAt)
            .Select(ArticleMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ArticleDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var dto = await _db.Articles.Where(a => a.Id == id).Select(ArticleMappings.ToDto).FirstOrDefaultAsync();
        if (dto is null) throw AppException.NotFound("Khong tim thay bai viet");
        return Ok(ApiResponse<ArticleDto>.Ok(dto));
    }
}
