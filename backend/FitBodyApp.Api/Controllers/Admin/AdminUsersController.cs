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
[Route("api/admin/users")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminUsersController : ControllerBase
{
    private readonly FitBodyDbContext _db;

    public AdminUsersController(FitBodyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? q, [FromQuery] string? status, [FromQuery] string? role,
        [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(u => EF.Functions.ILike(u.FullName, $"%{q}%") || EF.Functions.ILike(u.Email, $"%{q}%"));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<UserStatus>(status, true, out var st))
            query = query.Where(u => u.Status == st);
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var r))
            query = query.Where(u => u.Role == r);

        var paging = new PagedRequest { Page = page, Limit = limit };
        var (items, meta) = await query.OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserDto(u.Id, u.FullName, u.Email, u.Phone, u.Role.ToString(), u.Status.ToString(), u.CreatedAt))
            .ToPagedResultAsync(paging.Page, paging.Limit);

        return Ok(ApiResponse<List<AdminUserDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var user = await _db.Users.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy người dùng");
        var dto = new AdminUserDto(user.Id, user.FullName, user.Email, user.Phone, user.Role.ToString(), user.Status.ToString(), user.CreatedAt);
        return Ok(ApiResponse<AdminUserDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff(CreateStaffUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw AppException.ValidationError("Thiếu thông tin bắt buộc");
        if (request.Password.Length < 8)
            throw AppException.ValidationError("Mật khẩu phải có ít nhất 8 ký tự");
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role) || role == UserRole.User)
            throw AppException.ValidationError("Role không hợp lệ");

        if (!User.IsInRole("SuperAdmin") && role != UserRole.Moderator)
            throw AppException.Forbidden("Chỉ SuperAdmin mới có thể tạo tài khoản Admin/SuperAdmin");

        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            throw AppException.Conflict("Email đã được sử dụng");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active,
            Role = role,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Users.Add(user);
        _db.UserSettings.Add(new UserSetting { UserId = user.Id });
        await _db.SaveChangesAsync();

        var dto = new AdminUserDto(user.Id, user.FullName, user.Email, user.Phone, user.Role.ToString(), user.Status.ToString(), user.CreatedAt);
        return CreatedAtAction(nameof(GetDetail), new { id = user.Id }, ApiResponse<AdminUserDto>.Ok(dto));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateUserStatusRequest request)
    {
        if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
            throw AppException.ValidationError("Status không hợp lệ");

        var user = await _db.Users.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy người dùng");
        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật trạng thái thành công" }));
    }

    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateUserRoleRequest request)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            throw AppException.ValidationError("Role không hợp lệ");

        var user = await _db.Users.FindAsync(id) ?? throw AppException.NotFound("Không tìm thấy người dùng");
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { message = "Cập nhật role thành công" }));
    }
}
