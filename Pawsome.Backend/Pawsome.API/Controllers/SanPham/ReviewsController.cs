using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetByProductAsync(productId, page, pageSize);
        return Ok(ApiResponse<PagedResult<ReviewDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ReviewRequestDto dto)
    {
        try
        {
            var result = await _reviewService.CreateAsync(CurrentUserId, dto);
            return Ok(ApiResponse<ReviewDto>.Ok(result, "Gửi đánh giá thành công, đang chờ kiểm duyệt."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReviewDto>.Fail(ex.Message));
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> GetChoDuyet()
    {
        var result = await _reviewService.GetChoDuyetAsync();
        return Ok(ApiResponse<List<ReviewDto>>.Ok(result));
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Duyet(int id)
    {
        try
        {
            var result = await _reviewService.DuyetAsync(id);
            return Ok(ApiResponse<ReviewDto>.Ok(result, "Đã duyệt đánh giá."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> TuChoi(int id)
    {
        try
        {
            var result = await _reviewService.TuChoiAsync(id);
            return Ok(ApiResponse<ReviewDto>.Ok(result, "Đã từ chối đánh giá."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewDto>.Fail(ex.Message));
        }
    }
}
