using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.API.Services.TaiKhoan;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Quản lý tài khoản, phân quyền. Phải gọi IUserService của Phần 1 - không tự viết
// data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminUserController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] string? trangThai = null)
    {
        var result = await _userService.GetAllUsersAsync(pageNumber, pageSize, keyword, trangThai);
        return Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetUserByIdAdminAsync(id);
        if (result == null)
            return NotFound(ApiResponse<AdminUserDto>.Fail("Không tìm thấy người dùng."));

        return Ok(ApiResponse<AdminUserDto>.Ok(result));
    }

    [HttpPut("{id}/lock")]
    public async Task<IActionResult> Lock(int id)
    {
        try
        {
            await _userService.SetLockStatusAsync(id, locked: true);
            return Ok(ApiResponse<object>.Ok(null!, "Đã khóa tài khoản."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/unlock")]
    public async Task<IActionResult> Unlock(int id)
    {
        try
        {
            await _userService.SetLockStatusAsync(id, locked: false);
            return Ok(ApiResponse<object>.Ok(null!, "Đã mở khóa tài khoản."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleRequestDto dto)
    {
        try
        {
            await _userService.ChangeRoleAsync(id, dto.RoleId);
            return Ok(ApiResponse<object>.Ok(null!, "Đã cập nhật vai trò."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
