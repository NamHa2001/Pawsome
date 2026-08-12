using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.Common.AuditLog;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.Services.GioHang;
using Pawsome.API.Services.SanPham;
using Pawsome.Domain.Entities.DonHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.DonHang;

public class OrderService : IOrderService
{
    private const string ChoXuLy = "cho_xu_ly";
    private const string DangXuLy = "dang_xu_ly";
    private const string DaGiaoVan = "da_giao_van";
    private const string DaGiao = "da_giao";
    private const string DaHuy = "da_huy";
    private const string ChoTraHang = "cho_tra_hang";
    private const string DaTraHang = "da_tra_hang";

    private static readonly HashSet<string> TrangThaiChoPhepHuy = new() { ChoXuLy, DangXuLy };

    private readonly PawsomeDbContext _dbContext;
    private readonly IProductService _productService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public OrderService(PawsomeDbContext dbContext, IProductService productService,
        IAuditLogService auditLogService, ICartService cartService, ICouponService couponService)
    {
        _dbContext = dbContext;
        _productService = productService;
        _auditLogService = auditLogService;
        _cartService = cartService;
        _couponService = couponService;
    }

    public async Task<OrderDto> CreateFromCartAsync(int userId, CreateOrderRequestDto dto)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || cart.CartItems.Count == 0)
            throw new InvalidOperationException("Giỏ hàng đang trống, không thể đặt hàng.");

        var address = await _dbContext.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);
        if (address == null)
            throw new InvalidOperationException("Địa chỉ giao hàng không hợp lệ.");

        // Kiểm tra tồn kho trước khi trừ, tránh bán vượt số lượng thực tế
        foreach (var item in cart.CartItems)
        {
            if (!item.Variant.DangKinhDoanh || item.Variant.SoLuongTon < item.SoLuong)
                throw new InvalidOperationException($"Sản phẩm '{item.Variant.Product.Ten} - {item.Variant.TenBienThe}' không đủ hàng.");
        }

        var tienHang = cart.CartItems.Sum(ci => ci.Variant.Gia * ci.SoLuong);

        decimal giamGia = 0;
        if (dto.CouponId.HasValue)
        {
            var coupon = await _dbContext.Coupons.FindAsync(dto.CouponId.Value);
            var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
            if (coupon == null
                || (coupon.NgayBatDau.HasValue && coupon.NgayBatDau.Value > homNay)
                || (coupon.NgayKetThuc.HasValue && coupon.NgayKetThuc.Value < homNay)
                || (coupon.SoLuong.HasValue && coupon.SoLuong.Value <= 0))
                throw new InvalidOperationException("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

            giamGia = coupon.LoaiGiam == "percent"
                ? Math.Round(tienHang * coupon.GiaTri / 100, 0)
                : coupon.GiaTri;
            giamGia = Math.Min(giamGia, tienHang);
            await _couponService.SuDungMaAsync(coupon.MaCode);
        }

        // Phí vận chuyển: tạm tính cố định theo tỉnh/thành nơi giao
        var phiVanChuyen = address.TinhThanh.Trim().Equals("Hồ Chí Minh", StringComparison.OrdinalIgnoreCase)
            || address.TinhThanh.Trim().Equals("Hà Nội", StringComparison.OrdinalIgnoreCase)
            ? 20000m : 35000m;

        var thanhTien = Math.Max(0, tienHang + phiVanChuyen - giamGia);

        var order = new Order
        {
            UserId = userId,
            AddressId = dto.AddressId,
            CouponId = dto.CouponId,
            NgayDat = DateTime.UtcNow,
            TienHang = tienHang,
            PhiVanChuyen = phiVanChuyen,
            GiamGia = giamGia,
            ThanhTien = thanhTien,
            TrangThai = ChoXuLy,
            DonViVanChuyen = dto.DonViVanChuyen,
            NgayCapNhat = DateTime.UtcNow,
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                VariantId = ci.VariantId,
                SoLuong = ci.SoLuong,
                DonGia = ci.Variant.Gia // chốt giá tại thời điểm mua
            }).ToList()
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Trừ tồn kho qua hàm dùng chung của Phần 2 (không tự UPDATE bảng product_variants)
        foreach (var item in cart.CartItems)
            await _productService.TruTonKhoAsync(item.VariantId, item.SoLuong);

        // Cộng PawPoints: 1 điểm / 10.000đ. Bảng diem_pawpoints trên users tự đồng bộ
        // qua trigger trg_pawpoints_sync_balance - KHÔNG tự sửa users.diem_pawpoints ở đây.
        var soDiem = (int)(thanhTien / 10000);
        if (soDiem > 0)
        {
            _dbContext.PawPointsTransactions.Add(new PawPointsTransaction
            {
                UserId = userId,
                OrderId = order.OrderId,
                SoDiem = soDiem,
                Loai = "earn",
                NgayGiaoDich = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }
        await _cartService.XoaSachGioHangAsync(userId);
        await _auditLogService.LogAsync(userId, "TAO_DON_HANG", "orders", order.OrderId,
            $"Đặt hàng thành công, thành tiền {thanhTien:N0}đ");

        return await GetByIdAsync(userId, order.OrderId) ?? throw new InvalidOperationException("Lỗi tạo đơn hàng.");
    }

    public async Task<PagedResult<OrderDto>> GetByUserAsync(int userId, OrderFilterRequestDto filter)
    {
        var query = _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .Where(o => o.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            query = query.Where(o => o.TrangThai == filter.TrangThai);

        query = (IOrderedQueryable<Order>)query.OrderByDescending(o => o.NgayDat);

        var tongSo = await query.CountAsync();
        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 50);

        var items = await query.Skip((trang - 1) * soDong).Take(soDong).ToListAsync();

        return new PagedResult<OrderDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    public async Task<OrderDto?> GetByIdAsync(int userId, int orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CancelAsync(int userId, int orderId, CancelOrderRequestDto dto)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        if (!TrangThaiChoPhepHuy.Contains(order.TrangThai))
            throw new InvalidOperationException("Đơn hàng đã giao vận, không thể tự hủy. Vui lòng liên hệ hỗ trợ để yêu cầu trả hàng.");

        order.TrangThai = DaHuy;

        // Hoàn kho qua hàm dùng chung của Phần 2
        foreach (var item in order.OrderItems)
            await _productService.HoanKhoAsync(item.VariantId, item.SoLuong);

        // Hoàn PawPoints đã cộng lúc đặt đơn (nếu có) - trigger tự đồng bộ lại số dư
        var giaoDichCong = await _dbContext.PawPointsTransactions
            .Where(t => t.OrderId == orderId && t.Loai == "earn")
            .SumAsync(t => (int?)t.SoDiem) ?? 0;
        if (giaoDichCong > 0)
        {
            _dbContext.PawPointsTransactions.Add(new PawPointsTransaction
            {
                UserId = userId,
                OrderId = orderId,
                SoDiem = -giaoDichCong,
                Loai = "earn", // ghi âm để hoàn tác điểm của chính giao dịch earn ở trên
                NgayGiaoDich = DateTime.UtcNow
            });
        }

        if (order.CouponId.HasValue)
        {
            var coupon = await _dbContext.Coupons.FindAsync(order.CouponId.Value);
            if (coupon != null)
                await _couponService.HoanLuotSuDungMaAsync(coupon.MaCode);
        }

        await _dbContext.SaveChangesAsync();
        await _auditLogService.LogAsync(userId, "HUY_DON_HANG", "orders", orderId, dto.LyDo);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateTrangThaiAsync(int orderId, string trangThaiMoi)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        order.TrangThai = trangThaiMoi;
        await _dbContext.SaveChangesAsync();

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order o) => new()
    {
        OrderId = o.OrderId,
        UserId = o.UserId,
        AddressId = o.AddressId,
        CouponId = o.CouponId,
        NgayDat = o.NgayDat,
        TienHang = o.TienHang,
        PhiVanChuyen = o.PhiVanChuyen,
        GiamGia = o.GiamGia,
        ThanhTien = o.ThanhTien,
        TrangThai = o.TrangThai,
        DonViVanChuyen = o.DonViVanChuyen,
        MaVanDon = o.MaVanDon,
        OrderItems = o.OrderItems.Select(oi => new OrderItemDto
        {
            OrderItemId = oi.OrderItemId,
            VariantId = oi.VariantId,
            TenSanPham = oi.Variant.Product.Ten,
            TenBienThe = oi.Variant.TenBienThe,
            SoLuong = oi.SoLuong,
            DonGia = oi.DonGia
        }).ToList()
    };

    //Admin/nhân viên vận chuyển gọi khi đơn được gửi đi
    public async Task<OrderDto> CapNhatVanDonAsync(int orderId, UpdateShippingRequestDto dto)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        order.DonViVanChuyen = dto.DonViVanChuyen;
        order.MaVanDon = dto.MaVanDon;
        order.TrangThai = DaGiaoVan;
        await _dbContext.SaveChangesAsync();

        return MapToDto(order);
    }

    // khách yêu cầu trả hàng, chỉ khi đơn đã giao (da_giao)
    public async Task<OrderDto> YeuCauTraHangAsync(int userId, int orderId, ReturnRequestDto dto)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        if (order.TrangThai != DaGiao)
            throw new InvalidOperationException("Chỉ có thể yêu cầu trả hàng đối với đơn đã giao thành công.");

        order.TrangThai = ChoTraHang;
        await _dbContext.SaveChangesAsync();
        await _auditLogService.LogAsync(userId, "YEU_CAU_TRA_HANG", "orders", orderId, dto.LyDo);

        return MapToDto(order);
    }

    // Admin duyệt/từ chối yêu cầu trả hàng 
    public async Task<OrderDto> DuyetTraHangAsync(int orderId, bool dongY)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        if (order.TrangThai != ChoTraHang)
            throw new InvalidOperationException("Đơn hàng không ở trạng thái chờ trả hàng.");

        if (dongY)
        {
            order.TrangThai = DaTraHang;

            // Hoàn kho
            foreach (var item in order.OrderItems)
                await _productService.HoanKhoAsync(item.VariantId, item.SoLuong);

            // Hoàn PawPoints đã cộng lúc đặt đơn
            var giaoDichCong = await _dbContext.PawPointsTransactions
                .Where(t => t.OrderId == orderId && t.Loai == "earn")
                .SumAsync(t => (int?)t.SoDiem) ?? 0;
            if (giaoDichCong > 0)
            {
                _dbContext.PawPointsTransactions.Add(new PawPointsTransaction
                {
                    UserId = order.UserId,
                    OrderId = orderId,
                    SoDiem = -giaoDichCong,
                    Loai = "earn",
                    NgayGiaoDich = DateTime.UtcNow
                });
            }
        }
        else
        {
            order.TrangThai = DaGiao; // từ chối, trả đơn về trạng thái đã giao như cũ
        }

        await _dbContext.SaveChangesAsync();
        return MapToDto(order);
    }
}