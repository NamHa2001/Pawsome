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
    [Route("api/gio-hang")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        private int UserId => int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        [HttpGet]
        public async Task<IActionResult> LayGioHang()
        {
            var data = await _cartService.LayGioHangAsync(UserId);
            return Ok(new ApiResponse<CartDto> { Success = true, Data = data });
        }
        [HttpPost("items")]
        public async Task<IActionResult> ThemSanPham([FromBody] AddCartItemDto dto)
        {
            var data = await _cartService.ThemSanPhamAsync(UserId, dto);
            return Ok(new ApiResponse<CartDto> { Success = true, Data = data, Message = "Đã thêm vào giỏ hàng" });
        }
        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> CapNhatSoLuong(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var data = await _cartService.CapNhatSoLuongAsync(UserId, cartItemId, dto);
            return Ok(new ApiResponse<CartDto> { Success = true, Data = data });
        }
        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> XoaSanPham(int cartItemId)
        {
            await _cartService.XoaSanPhamAsync(UserId, cartItemId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa sản phẩm khỏi giỏ hàng" });
        }
        [HttpDelete]
        public async Task<IActionResult> XoaSachGioHang()
        {
            await _cartService.XoaSachGioHangAsync(UserId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa toàn bộ giỏ hàng" });
        }
        [HttpPost("ap-dung-ma-giam-gia")]
        public async Task<IActionResult> ApDungMaGiamGia([FromBody] ApplyCouponDto dto)
        {
            var data = await _cartService.ApDungMaGiamGiaAsync(UserId, dto);
            return Ok(new ApiResponse<ApplyCouponResultDto>
            {
                Success = data.HopLe,
                Data = data,
                Message = data.ThongBao
            });
        }
    }
}