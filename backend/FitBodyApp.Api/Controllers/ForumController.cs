using FitBodyApp.Api.Middleware;
using FitBodyApp.Application.Common;
using FitBodyApp.Application.Community;
using FitBodyApp.Domain.Entities;
using FitBodyApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Api.Controllers;

[ApiController]
[Route("api/forum/posts")]
[Authorize]
public class ForumController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public ForumController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.ForumPosts
            .OrderByDescending(p => p.CreatedAt)
            .Select(ForumPostMappings.ToDto)
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<ForumPostDto>>.Ok(items, meta));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateForumPostRequest request)
    {
        var userId = User.GetUserId();
        var post = new ForumPost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            ImageFileId = request.ImageFileId,
            LikesCount = 0,
            CreatedAt = DateTime.UtcNow
        };
        _db.ForumPosts.Add(post);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetail), new { id = post.Id }, ApiResponse<object>.Ok(new { id = post.Id }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var dto = await _db.ForumPosts.Where(p => p.Id == id).Select(ForumPostMappings.ToDto).FirstOrDefaultAsync();
        if (dto is null) throw AppException.NotFound("Không tìm thấy bài đăng");
        return Ok(ApiResponse<ForumPostDto>.Ok(dto));
    }

    [HttpPost("{id:guid}/likes")]
    public async Task<IActionResult> Like(Guid id)
    {
        var userId = User.GetUserId();
        var post = await _db.ForumPosts.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy bài đăng");

        var already = await _db.PostLikes.AnyAsync(l => l.PostId == id && l.UserId == userId);
        if (already) throw AppException.Conflict("Bạn đã thích bài đăng này rồi");

        _db.PostLikes.Add(new PostLike { Id = Guid.NewGuid(), PostId = id, UserId = userId, CreatedAt = DateTime.UtcNow });
        post.LikesCount += 1;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { likesCount = post.LikesCount }));
    }

    [HttpDelete("{id:guid}/likes")]
    public async Task<IActionResult> Unlike(Guid id)
    {
        var userId = User.GetUserId();
        var post = await _db.ForumPosts.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy bài đăng");

        var like = await _db.PostLikes.FirstOrDefaultAsync(l => l.PostId == id && l.UserId == userId)
            ?? throw AppException.NotFound("Bạn chưa thích bài đăng này");

        _db.PostLikes.Remove(like);
        post.LikesCount = Math.Max(0, post.LikesCount - 1);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { likesCount = post.LikesCount }));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, CreateCommentRequest request)
    {
        var exists = await _db.ForumPosts.AnyAsync(p => p.Id == id);
        if (!exists) throw AppException.NotFound("Không tìm thấy bài đăng");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = id,
            UserId = User.GetUserId(),
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComments), new { id }, ApiResponse<object>.Ok(new { id = comment.Id }));
    }

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid id, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await _db.Comments.Include(c => c.User)
            .Where(c => c.PostId == id)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(c.Id, c.UserId, c.User.FullName, c.Content, c.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<CommentDto>>.Ok(items, meta));
    }
}
