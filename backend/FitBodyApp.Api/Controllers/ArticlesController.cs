using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            .Select(a => new ArticleDto(a.Id, a.Title, a.Content, a.Category, a.CoverImage, a.Author, a.PublishedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ArticleDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var article = await _db.Articles.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay bai viet");
        var dto = new ArticleDto(article.Id, article.Title, article.Content, article.Category, article.CoverImage, article.Author, article.PublishedAt);
        return Ok(ApiResponse<ArticleDto>.Ok(dto));
    }
}
