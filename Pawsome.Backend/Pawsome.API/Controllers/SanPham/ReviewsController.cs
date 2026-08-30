using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Chỉ còn action đọc công khai + gửi đánh giá của khách - phần kiểm duyệt (duyệt/từ chối) đã
// dời hẳn sang AdminReviewController (Phần 5), đúng ranh giới ở Pawsome_PhanChia.docx (mục
// "Kiểm duyệt đánh giá": Phần 2 cho gửi đánh giá, Phần 5 thực hiện duyệt/từ chối).
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

    // Endpoint công khai (không [Authorize]) nhưng vẫn cần biết "phiếu của tôi" nếu khách đã đăng
    // nhập - middleware xác thực vẫn chạy dù action không bắt buộc Authorize, nên User.Identity
    // vẫn có claim thật khi request có kèm JWT hợp lệ.
    private int? CurrentUserIdOrNull =>
        User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!) : null;

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetByProductAsync(productId, page, pageSize, CurrentUserIdOrNull);
        return Ok(ApiResponse<PagedResult<ReviewDto>>.Ok(result));
    }

    [HttpGet("noi-bat")]
    public async Task<IActionResult> GetNoiBat([FromQuery] int soLuong = 6)
    {
        var result = await _reviewService.GetNoiBatAsync(soLuong);
        return Ok(ApiResponse<List<ReviewDto>>.Ok(result));
    }

    [HttpGet("product/{productId}/co-the-danh-gia")]
    [Authorize]
    public async Task<IActionResult> CoTheDanhGia(int productId)
    {
        var result = await _reviewService.CoTheDanhGiaAsync(CurrentUserId, productId);
        return Ok(ApiResponse<bool>.Ok(result));
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

    [HttpPost("{id}/vote")]
    [Authorize]
    public async Task<IActionResult> Vote(int id, [FromBody] VoteReviewRequestDto dto)
    {
        try
        {
            var result = await _reviewService.VoteAsync(CurrentUserId, id, dto.HuuIch);
            return Ok(ApiResponse<ReviewDto>.Ok(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReviewDto>.Fail(ex.Message));
        }
    }
}
