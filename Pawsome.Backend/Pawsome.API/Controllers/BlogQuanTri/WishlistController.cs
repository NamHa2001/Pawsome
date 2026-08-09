using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.Services.BlogQuanTri;

namespace Pawsome.API.Controllers.BlogQuanTri;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _wishlistService.GetByUserAsync(CurrentUserId, page, pageSize);
        return Ok(ApiResponse<PagedResult<WishlistItemDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] WishlistRequestDto dto)
    {
        try
        {
            var result = await _wishlistService.AddAsync(CurrentUserId, dto.ProductId);
            return Ok(ApiResponse<WishlistItemDto>.Ok(result, "Đã thêm vào danh sách yêu thích."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<WishlistItemDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        try
        {
            await _wishlistService.RemoveAsync(CurrentUserId, productId);
            return Ok(ApiResponse<object>.Ok(new { }, "Đã xóa khỏi danh sách yêu thích."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
