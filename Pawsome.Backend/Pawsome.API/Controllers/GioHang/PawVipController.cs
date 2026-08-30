using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.Services.GioHang;

namespace Pawsome.API.Controllers.GioHang
{
    [ApiController]
    [Route("api/pawvip")]
    [Authorize]
    public class PawVipController : ControllerBase
    {
        private readonly IPawVipService _pawVipService;
        private readonly IPawVipPaymentService _pawVipPaymentService;

        public PawVipController(IPawVipService pawVipService, IPawVipPaymentService pawVipPaymentService)
        {
            _pawVipService = pawVipService;
            _pawVipPaymentService = pawVipPaymentService;
        }

        private int UserId => int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        [HttpGet("trang-thai")]
        public async Task<IActionResult> LayTrangThai()
        {
            var (tier, hetHan) = await _pawVipService.LayTrangThaiAsync(UserId);
            return Ok(new ApiResponse<PawVipStatusDto> { Success = true, Data = new PawVipStatusDto { Tier = tier, HetHan = hetHan } });
        }

        // Kích hoạt PawVip chỉ xảy ra sau khi thanh toán MoMo/VNPay thành công (IPN xác nhận),
        // không còn endpoint kích hoạt free trực tiếp - xem PawVipPaymentService.

        [HttpPost("thanh-toan/momo")]
        public async Task<IActionResult> TaoThanhToanMoMo([FromBody] CreatePawVipPaymentDto dto)
        {
            try
            {
                var result = await _pawVipPaymentService.CreateMoMoPaymentAsync(UserId, dto.Tier);
                return Ok(ApiResponse<DTOs.DonHang.CreatePaymentResultDto>.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<DTOs.DonHang.CreatePaymentResultDto>.Fail(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("thanh-toan/momo/ipn")]
        public async Task<IActionResult> MoMoIpn([FromBody] DTOs.DonHang.MoMoIpnRequestDto dto)
        {
            try
            {
                await _pawVipPaymentService.HandleMoMoIpnAsync(dto);
                return Ok();
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
        }

        [HttpPost("thanh-toan/vnpay")]
        public async Task<IActionResult> TaoThanhToanVnPay([FromBody] CreatePawVipPaymentDto dto)
        {
            try
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (clientIp == "::1" || string.IsNullOrEmpty(clientIp))
                    clientIp = "127.0.0.1";
                var result = await _pawVipPaymentService.CreateVnPayPaymentAsync(UserId, dto.Tier, clientIp);
                return Ok(ApiResponse<DTOs.DonHang.CreatePaymentResultDto>.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<DTOs.DonHang.CreatePaymentResultDto>.Fail(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("thanh-toan/vnpay/ipn")]
        public async Task<IActionResult> VnPayIpn()
        {
            var query = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var ok = await _pawVipPaymentService.HandleVnPayIpnAsync(query);

            return Ok(new DTOs.DonHang.VnPayIpnResponseDto
            {
                RspCode = ok ? "00" : "97",
                Message = ok ? "Confirm Success" : "Invalid signature"
            });
        }

        [HttpGet("thanh-toan/{id}")]
        public async Task<IActionResult> LayTrangThaiThanhToan(int id)
        {
            var result = await _pawVipPaymentService.GetStatusAsync(UserId, id);
            if (result == null)
                return NotFound(ApiResponse<PawVipPaymentStatusDto>.Fail("Không tìm thấy giao dịch."));
            return Ok(ApiResponse<PawVipPaymentStatusDto>.Ok(result));
        }
    }
}
