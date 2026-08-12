using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.Services.DonHang;

namespace Pawsome.API.Controllers.DonHang;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto dto)
    {
        try
        {
            var result = await _orderService.CreateFromCartAsync(CurrentUserId, dto);
            return Ok(ApiResponse<OrderDto>.Ok(result, "Đặt hàng thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] OrderFilterRequestDto filter)
    {
        var result = await _orderService.GetByUserAsync(CurrentUserId, filter);
        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(CurrentUserId, id);
        if (result == null)
            return NotFound(ApiResponse<OrderDto>.Fail("Không tìm thấy đơn hàng."));

        return Ok(ApiResponse<OrderDto>.Ok(result));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderRequestDto dto)
    {
        try
        {
            var result = await _orderService.CancelAsync(CurrentUserId, id, dto);
            return Ok(ApiResponse<OrderDto>.Ok(result, "Hủy đơn hàng thành công."));
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

    [HttpPost("{id}/return-request")]
    public async Task<IActionResult> RequestReturn(int id, [FromBody] ReturnRequestDto dto)
    {
        try
        {
            var result = await _orderService.YeuCauTraHangAsync(CurrentUserId, id, dto);
            return Ok(ApiResponse<OrderDto>.Ok(result, "Đã gửi yêu cầu trả hàng, chờ duyệt."));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<OrderDto>.Fail(ex.Message)); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<OrderDto>.Fail(ex.Message)); }
    }
}