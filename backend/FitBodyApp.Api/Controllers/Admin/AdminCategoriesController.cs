using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminCategoriesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!Enum.TryParse<CategoryType>(type, true, out var categoryType))
                throw AppException.ValidationError("Type không hợp lệ");
            query = query.Where(c => c.Type == categoryType);
        }

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (categories, meta) = await query.OrderBy(c => c.Type).ThenBy(c => c.Name).ToPagedResultAsync(paging.Page, paging.Limit);

        var items = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var usageCount = await CountUsageAsync(category.Type, category.Name);
            items.Add(new CategoryDto(category.Id, category.Name, category.Type.ToString(), usageCount, category.CreatedAt));
        }

        return Ok(ApiResponse<List<CategoryDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw AppException.ValidationError("Tên category không được để trống");
        if (!Enum.TryParse<CategoryType>(request.Type, true, out var type))
            throw AppException.ValidationError("Type không hợp lệ");

        var name = request.Name.Trim();
        var exists = await _db.Categories.AnyAsync(c => c.Type == type && c.Name == name);
        if (exists)
            throw AppException.Conflict("Category này đã tồn tại");

        var now = DateTime.UtcNow;
        var category = new Category { Id = Guid.NewGuid(), Name = name, Type = type, CreatedAt = now, UpdatedAt = now };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = category.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw AppException.ValidationError("Tên category không được để trống");

        var category = await _db.Categories.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy category");
        var newName = request.Name.Trim();

        if (newName != category.Name)
        {
            var duplicate = await _db.Categories.AnyAsync(c => c.Type == category.Type && c.Name == newName && c.Id != id);
            if (duplicate)
                throw AppException.Conflict("Category này đã tồn tại");

            await RenameUsagesAsync(category.Type, category.Name, newName);
        }

        category.Name = newName;
        category.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _db.Categories.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy category");

        var usageCount = await CountUsageAsync(category.Type, category.Name);
        if (usageCount > 0)
            throw AppException.Conflict($"Category đang được sử dụng bởi {usageCount} nội dung, vui lòng đổi category cho các nội dung này trước");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private Task<int> CountUsageAsync(CategoryType type, string name) => type switch
    {
        CategoryType.Article => _db.Articles.CountAsync(a => a.Category == name),
        CategoryType.Video => _db.Videos.CountAsync(v => v.Category == name),
        CategoryType.Faq => _db.Faqs.CountAsync(f => f.Category == name),
        CategoryType.Food => _db.FoodItems.CountAsync(f => f.Category == name),
        _ => Task.FromResult(0)
    };

    private async Task RenameUsagesAsync(CategoryType type, string oldName, string newName)
    {
        switch (type)
        {
            case CategoryType.Article:
                await _db.Articles.Where(a => a.Category == oldName).ExecuteUpdateAsync(s => s.SetProperty(a => a.Category, newName));
                break;
            case CategoryType.Video:
                await _db.Videos.Where(v => v.Category == oldName).ExecuteUpdateAsync(s => s.SetProperty(v => v.Category, newName));
                break;
            case CategoryType.Faq:
                await _db.Faqs.Where(f => f.Category == oldName).ExecuteUpdateAsync(s => s.SetProperty(f => f.Category, newName));
                break;
            case CategoryType.Food:
                await _db.FoodItems.Where(f => f.Category == oldName).ExecuteUpdateAsync(s => s.SetProperty(f => f.Category, newName));
                break;
        }
    }
}
