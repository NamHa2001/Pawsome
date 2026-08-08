using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.API.Services.TaiKhoan;

namespace Pawsome.API.Controllers.TaiKhoan;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetProfileAsync(CurrentUserId);
        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto dto)
    {
        var result = await _userService.UpdateProfileAsync(CurrentUserId, dto);
        return Ok(ApiResponse<UserProfileDto>.Ok(result, "Cập nhật hồ sơ thành công."));
    }
}