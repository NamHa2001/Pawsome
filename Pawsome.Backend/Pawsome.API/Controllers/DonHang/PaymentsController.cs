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
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [Authorize]
    [HttpPost("momo/create-order/{orderId}")]
    public async Task<IActionResult> CreateMoMoPayment(int orderId)
    {
        try
        {
            var result = await _paymentService.CreateMoMoPaymentAsync(CurrentUserId, orderId);
            return Ok(ApiResponse<CreatePaymentResultDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<CreatePaymentResultDto>.Fail(ex.Message)); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<CreatePaymentResultDto>.Fail(ex.Message)); }
    }

    [AllowAnonymous]
    [HttpPost("momo/ipn")]
    public async Task<IActionResult> MoMoIpn([FromBody] MoMoIpnRequestDto dto)
    {
        try
        {
            await _paymentService.HandleMoMoIpnAsync(dto);
            return Ok();
        }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
    }

    [Authorize]
    [HttpPost("vnpay/create-order/{orderId}")]
    public async Task<IActionResult> CreateVnPayPayment(int orderId)
    {
        try
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (clientIp == "::1" || string.IsNullOrEmpty(clientIp))
                clientIp = "127.0.0.1";
            var result = await _paymentService.CreateVnPayPaymentAsync(CurrentUserId, orderId, clientIp);
            return Ok(ApiResponse<CreatePaymentResultDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<CreatePaymentResultDto>.Fail(ex.Message)); }
    }

    [AllowAnonymous]
    [HttpGet("vnpay/ipn")]
    public async Task<IActionResult> VnPayIpn()
    {
        var query = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var ok = await _paymentService.HandleVnPayIpnAsync(query);

        return Ok(new VnPayIpnResponseDto
        {
            RspCode = ok ? "00" : "97",
            Message = ok ? "Confirm Success" : "Invalid signature"
        });
    }

    [Authorize]
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var result = await _paymentService.GetByOrderIdAsync(CurrentUserId, orderId);
        if (result == null)
            return NotFound(ApiResponse<PaymentDto>.Fail("Chưa có giao dịch thanh toán nào cho đơn này."));
        return Ok(ApiResponse<PaymentDto>.Ok(result));
    }
}