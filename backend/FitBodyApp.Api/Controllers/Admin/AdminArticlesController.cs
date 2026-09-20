using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Content;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/articles")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminArticlesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminArticlesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Articles.OrderByDescending(a => a.PublishedAt)
            .Select(ArticleMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<ArticleDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertArticleRequest request)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            CoverImageId = request.CoverImageId,
            Author = request.Author,
            PublishedAt = DateTime.UtcNow
        };
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = article.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertArticleRequest request)
    {
        var article = await _db.Articles.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy bài viết");
        article.Title = request.Title;
        article.Content = request.Content;
        article.Category = request.Category;
        article.CoverImageId = request.CoverImageId;
        article.Author = request.Author;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var article = await _db.Articles.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy bài viết");
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
