using Pawsome.API.DTOs.GioHang;

namespace Pawsome.API.Services.GioHang
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> LayDanhSachDangHieuLucAsync();
        Task<CouponDto?> KiemTraMaHopLeAsync(string maCode);

        Task<IEnumerable<CouponDto>> LayTatCaAsync();
        Task<CouponDto> TaoAsync(CreateCouponDto dto);
        Task<CouponDto> CapNhatAsync(int id, UpdateCouponDto dto);
        Task XoaAsync(int id);

        // Dùng chung cho Phần 4 khi chốt đơn/hủy đơn có áp mã giảm giá. Nếu so_luong = NULL
        // (không giới hạn lượt dùng) thì không trừ/hoàn.
        Task SuDungMaAsync(string maCode);
        Task HoanLuotSuDungMaAsync(string maCode);
    }
}