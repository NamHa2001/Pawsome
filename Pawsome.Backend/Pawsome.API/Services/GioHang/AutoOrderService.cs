using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    public class AutoOrderService : IAutoOrderService
    {
        private readonly PawsomeDbContext _context;
        private readonly IProductService _productService;

        public AutoOrderService(PawsomeDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<IEnumerable<AutoOrderDto>> LayDanhSachAsync(int userId)
        {
            var list = await _context.AutoOrders
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.NgayKeTiep)
                .ToListAsync();

            var result = new List<AutoOrderDto>();
            foreach (var a in list)
            {
                var variant = await _productService.LayThongTinBienTheAsync(a.VariantId);
                result.Add(MapToDto(a, variant));
            }

            return result;
        }

        public async Task<AutoOrderDto> TaoAsync(int userId, CreateAutoOrderDto dto)
        {
            var variant = await _productService.LayThongTinBienTheAsync(dto.VariantId)
                ?? throw new KeyNotFoundException("Biến thể sản phẩm không tồn tại");

            var autoOrder = new AutoOrder
            {
                UserId = userId,
                VariantId = dto.VariantId,
                SoLuong = dto.SoLuong,
                TanSuat = dto.TanSuat,
                NgayKeTiep = TinhNgayKeTiep(DateOnly.FromDateTime(DateTime.Now), dto.TanSuat),
                TrangThai = "active"
            };

            _context.AutoOrders.Add(autoOrder);
            await _context.SaveChangesAsync();

            return MapToDto(autoOrder, variant);
        }

        public async Task<AutoOrderDto> CapNhatAsync(int userId, int id, UpdateAutoOrderDto dto)
        {
            var autoOrder = await LayCuaUserAsync(userId, id);

            autoOrder.SoLuong = dto.SoLuong;
            autoOrder.TanSuat = dto.TanSuat;       
            autoOrder.NgayKeTiep = TinhNgayKeTiep(DateOnly.FromDateTime(DateTime.Now), dto.TanSuat);

            await _context.SaveChangesAsync();

            var variant = await _productService.LayThongTinBienTheAsync(autoOrder.VariantId);
            return MapToDto(autoOrder, variant);
        }

        public async Task TamDungAsync(int userId, int id)
        {
            var autoOrder = await LayCuaUserAsync(userId, id);
            autoOrder.TrangThai = "paused";
            await _context.SaveChangesAsync();
        }

        public async Task KichHoatAsync(int userId, int id)
        {
            var autoOrder = await LayCuaUserAsync(userId, id);
            autoOrder.TrangThai = "active";
            autoOrder.NgayKeTiep = TinhNgayKeTiep(DateOnly.FromDateTime(DateTime.Now), autoOrder.TanSuat);
            await _context.SaveChangesAsync();
        }

        public async Task HuyAsync(int userId, int id)
        {
            var autoOrder = await LayCuaUserAsync(userId, id);
            autoOrder.TrangThai = "cancelled";
            await _context.SaveChangesAsync();
        }

        private async Task<AutoOrder> LayCuaUserAsync(int userId, int id)
        {
            var autoOrder = await _context.AutoOrders
                .FirstOrDefaultAsync(a => a.AutoOrderId == id && a.UserId == userId);

            return autoOrder ?? throw new KeyNotFoundException("Không tìm thấy đơn đặt hàng tự động");
        }

        private static DateOnly TinhNgayKeTiep(DateOnly tuNgay, string tanSuat) => tanSuat switch
        {
            "weekly" => tuNgay.AddDays(7),
            "monthly" => tuNgay.AddMonths(1),
            "quarterly" => tuNgay.AddMonths(3),
            "yearly" => tuNgay.AddYears(1),
            _ => tuNgay.AddMonths(1)
        };

        private static AutoOrderDto MapToDto(AutoOrder a, ProductVariantInfoDto? variant) => new()
        {
            AutoOrderId = a.AutoOrderId,
            VariantId = a.VariantId,
            TenSanPham = variant?.TenSanPham ?? "(Sản phẩm không còn tồn tại)",
            HinhAnh = variant?.HinhAnhChinh,
            SoLuong = a.SoLuong,
            TanSuat = a.TanSuat,
            NgayKeTiep = a.NgayKeTiep,
            TrangThai = a.TrangThai
        };
    }
}