using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common.AuditLog;
using Pawsome.API.Common.Email;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.DonHang;
using Pawsome.API.Services.SanPham;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    public class AutoOrderService : IAutoOrderService
    {
        private readonly PawsomeDbContext _context;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public AutoOrderService(PawsomeDbContext context, IProductService productService,
            IOrderService orderService, IEmailService emailService, IAuditLogService auditLogService)
        {
            _context = context;
            _productService = productService;
            _orderService = orderService;
            _emailService = emailService;
            _auditLogService = auditLogService;
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

        public async Task<int> XuLyDonDenHanAsync()
        {
            var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
            var denHan = await _context.AutoOrders
                .Include(a => a.User)
                .Include(a => a.Variant).ThenInclude(v => v.Product)
                .Where(a => a.TrangThai == "active" && a.NgayKeTiep <= homNay)
                .ToListAsync();

            var soDonTaoDuoc = 0;
            foreach (var a in denHan)
            {
                var order = await _orderService.TaoDonTuAutoOrderAsync(a.UserId, a.VariantId, a.SoLuong);
                if (order == null)
                {
                    // Hết hàng hoặc chưa có địa chỉ giao hàng - giữ nguyên NgayKeTiep để thử lại
                    // ở lần chạy kế tiếp (job chạy 1 lần/ngày nên không lặp vô hạn trong ngày).
                    await _auditLogService.LogAsync(a.UserId, "AUTO_ORDER_THAT_BAI", "auto_orders", a.AutoOrderId,
                        "Không tạo được đơn tự động: hết hàng hoặc chưa có địa chỉ giao hàng");
                    continue;
                }

                a.NgayKeTiep = TinhNgayKeTiep(homNay, a.TanSuat);
                await _context.SaveChangesAsync();
                soDonTaoDuoc++;

                if (!string.IsNullOrWhiteSpace(a.User.Email))
                {
                    var tenSp = $"{a.Variant.Product.Ten} - {a.Variant.TenBienThe}";
                    await _emailService.SendAsync(a.User.Email, "Đơn đặt hàng tự động đã được tạo - Pawsome",
                        $"Xin chào {a.User.HoTen},<br><br>" +
                        $"Đơn đặt hàng tự động của bạn cho sản phẩm <b>{tenSp}</b> (số lượng {a.SoLuong}) " +
                        $"đã được tạo thành công, mã đơn #{order.OrderId}, thành tiền {order.ThanhTien:N0}đ.<br><br>" +
                        "Vui lòng đăng nhập Pawsome, vào mục Lịch sử đơn hàng để hoàn tất thanh toán cho đơn này.<br><br>" +
                        "Trân trọng,<br>Pawsome");
                }
            }

            return soDonTaoDuoc;
        }

        public async Task<int> GuiNhacNhoTruocHanAsync(int soNgayTruoc)
        {
            var ngayNhac = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(soNgayTruoc);
            var sapToiHan = await _context.AutoOrders
                .Include(a => a.User)
                .Include(a => a.Variant).ThenInclude(v => v.Product)
                .Where(a => a.TrangThai == "active" && a.NgayKeTiep == ngayNhac)
                .ToListAsync();

            var soDaGui = 0;
            foreach (var a in sapToiHan)
            {
                if (string.IsNullOrWhiteSpace(a.User.Email)) continue;

                var tenSp = $"{a.Variant.Product.Ten} - {a.Variant.TenBienThe}";
                await _emailService.SendAsync(a.User.Email, "Nhắc nhở: đơn đặt hàng tự động sắp tới hạn - Pawsome",
                    $"Xin chào {a.User.HoTen},<br><br>" +
                    $"Đơn đặt hàng tự động cho sản phẩm <b>{tenSp}</b> (số lượng {a.SoLuong}) sẽ được tạo " +
                    $"vào ngày {a.NgayKeTiep:dd/MM/yyyy}. Nếu không muốn tiếp tục, bạn có thể tạm dừng hoặc " +
                    "hủy trong mục Đặt hàng tự động trước ngày trên.<br><br>" +
                    "Trân trọng,<br>Pawsome");
                soDaGui++;
            }

            return soDaGui;
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