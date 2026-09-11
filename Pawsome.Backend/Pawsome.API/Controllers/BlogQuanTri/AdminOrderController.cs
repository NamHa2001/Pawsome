using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.Services.BlogQuanTri;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Xem/cập nhật trạng thái đơn hàng. Phải gọi IOrderService của Phần 4 - không tự
// viết data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminOrderController : ControllerBase
{
    private readonly IAdminOrderService _adminOrderService;

    public AdminOrderController(IAdminOrderService adminOrderService)
    {
        _adminOrderService = adminOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterRequestDto filter)
    {
        var result = await _adminOrderService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _adminOrderService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<OrderDto>.Fail("Không tìm thấy đơn hàng."));

        return Ok(ApiResponse<OrderDto>.Ok(result));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequestDto dto)
    {
        try
        {
            var result = await _adminOrderService.UpdateStatusAsync(id, dto.TrangThaiMoi);
            return Ok(ApiResponse<OrderDto>.Ok(result, "Đã cập nhật trạng thái đơn hàng."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<OrderDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/return-decision")]
    public async Task<IActionResult> DuyetTraHang(int id, [FromBody] ReturnDecisionRequestDto dto)
    {
        try
        {
            var result = await _adminOrderService.DuyetTraHangAsync(id, dto.DongY);
            return Ok(ApiResponse<OrderDto>.Ok(result,
                dto.DongY ? "Đã duyệt yêu cầu trả hàng." : "Đã từ chối yêu cầu trả hàng."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<OrderDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/shipping")]
    public async Task<IActionResult> CapNhatVanDon(int id, [FromBody] UpdateShippingRequestDto dto)
    {
        try
        {
            var result = await _adminOrderService.CapNhatVanDonAsync(id, dto);
            return Ok(ApiResponse<OrderDto>.Ok(result, "Đã cập nhật thông tin vận chuyển."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<OrderDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.Fail(ex.Message));
        }
    }
}
