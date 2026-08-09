using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Kiểm duyệt đánh giá (duyệt/từ chối). Phải gọi IReviewService của Phần 2 - không
// tự viết data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
// Quyền: Admin hoặc Moderator (đúng Pawsome_PhanChia.docx - "do vai trò Người kiểm
// duyệt/Quản trị thực hiện").
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class AdminReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetChoDuyet()
    {
        var result = await _reviewService.GetChoDuyetAsync();
        return Ok(ApiResponse<List<ReviewDto>>.Ok(result));
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
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
    public async Task<IActionResult> Reject(int id)
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
