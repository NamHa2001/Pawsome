using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.GioHang;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    public class CouponService : ICouponService
    {
        private readonly PawsomeDbContext _context;

        public CouponService(PawsomeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CouponDto>> LayDanhSachDangHieuLucAsync()
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            var coupons = await _context.Coupons
                .Where(c =>
                    (c.NgayBatDau == null || c.NgayBatDau <= homNay) &&
                    (c.NgayKetThuc == null || c.NgayKetThuc >= homNay) &&
                    (c.SoLuong == null || c.SoLuong > 0))
                .ToListAsync();

            return coupons.Select(c => MapToDto(c, true));
        }

        public async Task<CouponDto?> KiemTraMaHopLeAsync(string maCode)
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.MaCode == maCode);

            if (coupon == null) return null;

            var hopLe =
                (coupon.NgayBatDau == null || coupon.NgayBatDau <= homNay) &&
                (coupon.NgayKetThuc == null || coupon.NgayKetThuc >= homNay) &&
                (coupon.SoLuong == null || coupon.SoLuong > 0);

            return hopLe ? MapToDto(coupon, true) : null;
        }

        public async Task<IEnumerable<CouponDto>> LayTatCaAsync()
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);
            var coupons = await _context.Coupons.ToListAsync();

            return coupons.Select(c => MapToDto(c,
                (c.NgayBatDau == null || c.NgayBatDau <= homNay) &&
                (c.NgayKetThuc == null || c.NgayKetThuc >= homNay) &&
                (c.SoLuong == null || c.SoLuong > 0)));
        }

        public async Task<CouponDto> TaoAsync(CreateCouponDto dto)
        {
            var trung = await _context.Coupons.AnyAsync(c => c.MaCode == dto.MaCode);
            if (trung)
                throw new InvalidOperationException("Mã giảm giá đã tồn tại");

            var coupon = new Coupon
            {
                MaCode = dto.MaCode,
                LoaiGiam = dto.LoaiGiam,
                GiaTri = dto.GiaTri,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                SoLuong = dto.SoLuong
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return MapToDto(coupon, true);
        }

        public async Task<CouponDto> CapNhatAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _context.Coupons.FindAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy mã giảm giá");

            coupon.LoaiGiam = dto.LoaiGiam;
            coupon.GiaTri = dto.GiaTri;
            coupon.NgayBatDau = dto.NgayBatDau;
            coupon.NgayKetThuc = dto.NgayKetThuc;
            coupon.SoLuong = dto.SoLuong;

            await _context.SaveChangesAsync();

            return MapToDto(coupon, true);
        }

        public async Task XoaAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy mã giảm giá");
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        private static CouponDto MapToDto(Coupon c, bool dangHieuLuc) => new()
        {
            CouponId = c.CouponId,
            MaCode = c.MaCode,
            LoaiGiam = c.LoaiGiam,
            GiaTri = c.GiaTri,
            NgayBatDau = c.NgayBatDau,
            NgayKetThuc = c.NgayKetThuc,
            SoLuong = c.SoLuong,
            DangHieuLuc = dangHieuLuc
        };
        public async Task SuDungMaAsync(string maCode)
        {
            var coupon = await _context.Coupons.AsNoTracking()
                .FirstOrDefaultAsync(c => c.MaCode == maCode)
                ?? throw new KeyNotFoundException("Mã giảm giá không tồn tại");

            if (coupon.SoLuong == null) return; // không giới hạn lượt dùng

            var soDongCapNhat = await _context.Coupons
                .Where(c => c.MaCode == maCode && c.SoLuong > 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.SoLuong, c => c.SoLuong - 1));

            if (soDongCapNhat == 0)
                throw new InvalidOperationException("Mã giảm giá đã hết lượt sử dụng");
        }

        public async Task HoanLuotSuDungMaAsync(string maCode)
        {
            await _context.Coupons
                .Where(c => c.MaCode == maCode && c.SoLuong != null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.SoLuong, c => c.SoLuong + 1));
        }
    }
}