using Pawsome.API.DTOs.GioHang;

namespace Pawsome.API.Services.GioHang
{
    public interface ICartService
    {
        Task<CartDto> LayGioHangAsync(int userId);
        Task<CartDto> ThemSanPhamAsync(int userId, AddCartItemDto dto);
        Task<CartDto> CapNhatSoLuongAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task XoaSanPhamAsync(int userId, int cartItemId);
        Task XoaSachGioHangAsync(int userId);
        Task<ApplyCouponResultDto> ApDungMaGiamGiaAsync(int userId, ApplyCouponDto dto);
        Task<CartDto> XoaMaGiamGiaAsync(int userId);
    }
}