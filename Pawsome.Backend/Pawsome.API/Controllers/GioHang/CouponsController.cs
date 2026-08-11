using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.Services.GioHang;

namespace Pawsome.API.Controllers.GioHang
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("dang-hieu-luc")]
        [AllowAnonymous]
        public async Task<IActionResult> LayDangHieuLuc()
        {
            var data = await _couponService.LayDanhSachDangHieuLucAsync();
            return Ok(new ApiResponse<IEnumerable<CouponDto>> { Success = true, Data = data });
        }

        [HttpGet("kiem-tra/{maCode}")]
        [Authorize]
        public async Task<IActionResult> KiemTraMa(string maCode)
        {
            var coupon = await _couponService.KiemTraMaHopLeAsync(maCode);
            if (coupon == null)
            {
                return Ok(new ApiResponse<CouponDto?>
                {
                    Success = false,
                    Message = "Mã giảm giá không hợp lệ hoặc đã hết hạn"
                });
            }

            return Ok(new ApiResponse<CouponDto?> { Success = true, Data = coupon });
        }

        [HttpGet]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> LayTatCa()
        {
            var data = await _couponService.LayTatCaAsync();
            return Ok(new ApiResponse<IEnumerable<CouponDto>> { Success = true, Data = data });
        }

        [HttpPost]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> Tao([FromBody] CreateCouponDto dto)
        {
            var data = await _couponService.TaoAsync(dto);
            return Ok(new ApiResponse<CouponDto> { Success = true, Data = data, Message = "Tạo mã giảm giá thành công" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> CapNhat(int id, [FromBody] UpdateCouponDto dto)
        {
            var data = await _couponService.CapNhatAsync(id, dto);
            return Ok(new ApiResponse<CouponDto> { Success = true, Data = data, Message = "Cập nhật mã giảm giá thành công" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> Xoa(int id)
        {
            await _couponService.XoaAsync(id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa mã giảm giá" });
        }
    }
}