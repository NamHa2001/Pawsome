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
    [Route("api/dat-hang-tu-dong")]
    [Authorize]
    public class AutoOrdersController : ControllerBase
    {
        private readonly IAutoOrderService _autoOrderService;
        public AutoOrdersController(IAutoOrderService autoOrderService)
        {
            _autoOrderService = autoOrderService;
        }
        private int UserId => int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        [HttpGet]
        public async Task<IActionResult> LayDanhSach()
        {
            var data = await _autoOrderService.LayDanhSachAsync(UserId);
            return Ok(new ApiResponse<IEnumerable<AutoOrderDto>> { Success = true, Data = data });
        }
        [HttpPost]
        public async Task<IActionResult> Tao([FromBody] CreateAutoOrderDto dto)
        {
            var data = await _autoOrderService.TaoAsync(UserId, dto);
            return Ok(new ApiResponse<AutoOrderDto>
            {
                Success = true,
                Data = data,
                Message = "Đã thiết lập đơn đặt hàng tự động"
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhat(int id, [FromBody] UpdateAutoOrderDto dto)
        {
            var data = await _autoOrderService.CapNhatAsync(UserId, id, dto);
            return Ok(new ApiResponse<AutoOrderDto> { Success = true, Data = data, Message = "Cập nhật đơn tự động thành công" });
        }
        [HttpPatch("{id}/tam-dung")]
        public async Task<IActionResult> TamDung(int id)
        {
            await _autoOrderService.TamDungAsync(UserId, id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã tạm dừng đơn đặt hàng tự động" });
        }
        [HttpPatch("{id}/kich-hoat")]
        public async Task<IActionResult> KichHoat(int id)
        {
            await _autoOrderService.KichHoatAsync(UserId, id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã kích hoạt lại đơn đặt hàng tự động" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Huy(int id)
        {
            await _autoOrderService.HuyAsync(UserId, id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã hủy đơn đặt hàng tự động" });
        }
    }
}