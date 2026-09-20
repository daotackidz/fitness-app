using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Community;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Domain.Enums;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/challenges")]
[Authorize]
public class ChallengesController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public ChallengesController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Challenges.AsQueryable();
        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<ChallengeType>(type, true, out var challengeType))
            query = query.Where(c => c.Type == challengeType);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderBy(c => c.StartDate)
            .Select(c => new ChallengeDto(c.Id, c.Name, c.Description, c.Type.ToString(), c.StartDate, c.EndDate, c.GoalMetric, c.Reward,
                c.ImageFile != null ? c.ImageFile.Url : null, null))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ChallengeDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var challenge = await _db.Challenges
            .Include(c => c.Participants).ThenInclude(p => p.User)
            .Include(c => c.ImageFile)
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw AppException.NotFound("Không tìm thấy thử thách");

        var leaderboard = challenge.Participants.OrderByDescending(p => p.Progress)
            .Select(p => new ChallengeParticipantDto(p.UserId, p.User.FullName, p.Progress, p.Rank, p.JoinedAt))
            .ToList();

        var dto = new ChallengeDto(challenge.Id, challenge.Name, challenge.Description, challenge.Type.ToString(),
            challenge.StartDate, challenge.EndDate, challenge.GoalMetric, challenge.Reward,
            challenge.ImageFile != null ? challenge.ImageFile.Url : null, leaderboard);
        return Ok(ApiResponse<ChallengeDto>.Ok(dto));
    }

    [HttpPost("{id:guid}/participants")]
    public async Task<IActionResult> Join(Guid id)
    {
        var exists = await _db.Challenges.AnyAsync(c => c.Id == id);
        if (!exists) throw AppException.NotFound("Không tìm thấy thử thách");

        var userId = User.GetUserId();
        var already = await _db.ChallengeParticipants.AnyAsync(p => p.ChallengeId == id && p.UserId == userId);
        if (already) throw AppException.Conflict("Bạn đã tham gia thử thách này rồi");

        var participant = new ChallengeParticipant
        {
            Id = Guid.NewGuid(),
            ChallengeId = id,
            UserId = userId,
            Progress = 0,
            JoinedAt = DateTime.UtcNow
        };
        _db.ChallengeParticipants.Add(participant);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetail), new { id }, ApiResponse<object>.Ok(new { id = participant.Id }));
    }

    [HttpPatch("{id:guid}/participants/me")]
    public async Task<IActionResult> UpdateMyProgress(Guid id, UpdateParticipantProgressRequest request)
    {
        var userId = User.GetUserId();
        var participant = await _db.ChallengeParticipants.FirstOrDefaultAsync(p => p.ChallengeId == id && p.UserId == userId)
            ?? throw AppException.NotFound("Bạn chưa tham gia thử thách này");

        participant.Progress = request.Progress;
        await _db.SaveChangesAsync();

        var ranked = await _db.ChallengeParticipants.Where(p => p.ChallengeId == id)
            .OrderByDescending(p => p.Progress).ToListAsync();
        for (int i = 0; i < ranked.Count; i++) ranked[i].Rank = i + 1;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { progress = participant.Progress, rank = participant.Rank }));
    }
}
