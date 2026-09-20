using FitBodyApp.Application.Common;
using FitBodyApp.Application.Support;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/faqs")]
[Authorize]
public class FaqsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public FaqsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? category, [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Faqs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(f => f.Category == category);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(f => EF.Functions.ILike(f.Question, $"%{q}%"));

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(f => f.Question)
            .Select(f => new FaqDto(f.Id, f.Question, f.Answer, f.Category))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<FaqDto>>.Ok(items, meta));
    }
}
