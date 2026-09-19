using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Support;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/faqs")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminFaqsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminFaqsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Faqs.OrderBy(f => f.Question)
            .Select(f => new FaqDto(f.Id, f.Question, f.Answer, f.Category))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<FaqDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertFaqRequest request)
    {
        var faq = new Faq { Id = Guid.NewGuid(), Question = request.Question, Answer = request.Answer, Category = request.Category };
        _db.Faqs.Add(faq);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = faq.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertFaqRequest request)
    {
        var faq = await _db.Faqs.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay FAQ");
        faq.Question = request.Question;
        faq.Answer = request.Answer;
        faq.Category = request.Category;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cap nhat thanh cong" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var faq = await _db.Faqs.FindAsync(id) ?? throw AppException.NotFound("Khong tim thay FAQ");
        _db.Faqs.Remove(faq);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
