using FitBodyApp.Application.Common;
using FitBodyApp.Application.Nutrition;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/foods")]
[Authorize]
public class FoodsController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public FoodsController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.FoodItems.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(f => EF.Functions.ILike(f.Name, $"%{q}%"));

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(f => f.Name)
            .Select(FoodItemMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<FoodItemDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var dto = await _db.FoodItems.Where(f => f.Id == id).Select(FoodItemMappings.ToDto).FirstOrDefaultAsync();
        if (dto is null) throw AppException.NotFound("Không tìm thấy thực phẩm");
        return Ok(ApiResponse<FoodItemDto>.Ok(dto));
    }
}
