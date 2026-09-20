using FitBodyApp.Application.Admin;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Community;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBodyApp.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/challenges")]
[Authorize(Roles = "Moderator,Admin,SuperAdmin")]
public class AdminChallengesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminChallengesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Challenges.OrderBy(c => c.StartDate)
            .Select(c => new ChallengeDto(c.Id, c.Name, c.Description, c.Type.ToString(), c.StartDate, c.EndDate, c.GoalMetric, c.Reward,
                c.ImageFile != null ? c.ImageFile.Url : null, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);
        return Ok(ApiResponse<List<ChallengeDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertChallengeRequest request)
    {
        if (!Enum.TryParse<ChallengeType>(request.Type, true, out var type))
            throw AppException.ValidationError("type không hợp lệ");

        var challenge = new Challenge
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            GoalMetric = request.GoalMetric,
            Reward = request.Reward,
            ImageFileId = request.ImageFileId
        };
        _db.Challenges.Add(challenge);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetList), null, ApiResponse<object>.Ok(new { id = challenge.Id }));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertChallengeRequest request)
    {
        var challenge = await _db.Challenges.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy thử thách");
        if (!Enum.TryParse<ChallengeType>(request.Type, true, out var type))
            throw AppException.ValidationError("type không hợp lệ");

        challenge.Name = request.Name;
        challenge.Description = request.Description;
        challenge.Type = type;
        challenge.StartDate = request.StartDate;
        challenge.EndDate = request.EndDate;
        challenge.GoalMetric = request.GoalMetric;
        challenge.Reward = request.Reward;
        challenge.ImageFileId = request.ImageFileId;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật thành công" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var challenge = await _db.Challenges.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy thử thách");
        _db.Challenges.Remove(challenge);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
