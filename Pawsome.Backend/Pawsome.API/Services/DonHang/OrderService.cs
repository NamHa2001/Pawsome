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
    private readonly PawsomeDbContext _dbContext;
    private readonly IProductService _productService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;
    private readonly IPawPointsService _pawPointsService;

    public OrderService(PawsomeDbContext dbContext, IProductService productService,
        IAuditLogService auditLogService, ICartService cartService, ICouponService couponService,
        IPawPointsService pawPointsService)
    {
        _dbContext = dbContext;
        _productService = productService;
        _auditLogService = auditLogService;
        _cartService = cartService;
        _couponService = couponService;
        _pawPointsService = pawPointsService;
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
        var homNay = DateOnly.FromDateTime(DateTime.UtcNow);

        decimal giamGiaCoupon = 0;
        if (dto.CouponId.HasValue)
        {
            var coupon = await _dbContext.Coupons.FindAsync(dto.CouponId.Value);
            if (coupon == null
                || (coupon.NgayBatDau.HasValue && coupon.NgayBatDau.Value > homNay)
                || (coupon.NgayKetThuc.HasValue && coupon.NgayKetThuc.Value < homNay)
                || (coupon.SoLuong.HasValue && coupon.SoLuong.Value <= 0))
                throw new InvalidOperationException("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

            giamGiaCoupon = coupon.LoaiGiam == "percent"
                ? Math.Round(tienHang * coupon.GiaTri / 100, 0)
                : coupon.GiaTri;
            giamGiaCoupon = Math.Min(giamGiaCoupon, tienHang);
        }

        decimal giamGiaDiem = 0;
        if (dto.SoDiemMuonDoi is > 0)
        {
            giamGiaDiem = await _pawPointsService.KiemTraVaTinhQuyDoiAsync(userId, dto.SoDiemMuonDoi.Value);
        }
        decimal giamGiaPawVip = 0;
        var pawVipInfo = await _dbContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => new { u.PawVipTier, u.PawVipHetHan })
            .FirstOrDefaultAsync();
        if (pawVipInfo != null
            && PawVipTiers.ConHieuLuc(pawVipInfo.PawVipTier, pawVipInfo.PawVipHetHan, homNay)
            && PawVipTiers.PhanTramGiam.TryGetValue(pawVipInfo.PawVipTier!, out var phanTramPawVip))
        {
            giamGiaPawVip = Math.Round(tienHang * phanTramPawVip, 0);
        }

        var giamGia = Math.Min(giamGiaCoupon + giamGiaDiem + giamGiaPawVip, tienHang);

        var mienPhiShipPawVip = pawVipInfo != null
            && PawVipTiers.ConHieuLuc(pawVipInfo.PawVipTier, pawVipInfo.PawVipHetHan, homNay)
            && PawVipTiers.TierMienPhiShip.Contains(pawVipInfo.PawVipTier!);

        // - "GHN" (Nhanh): phí cơ bản + phụ phí giao nhanh, giao nhanh hơn.
        var phiCoBanTheoTinh = address.TinhThanh.Trim().Equals("Hồ Chí Minh", StringComparison.OrdinalIgnoreCase)
            || address.TinhThanh.Trim().Equals("Hà Nội", StringComparison.OrdinalIgnoreCase)
            ? 20000m : 35000m;

        var phiVanChuyen = mienPhiShipPawVip
            ? 0m
            : dto.DonViVanChuyen switch
            {
                "Free" => 0m,
                "GHN" => phiCoBanTheoTinh + 15000m,
                "GHTK" => phiCoBanTheoTinh,
                _ => phiCoBanTheoTinh
            };

        var thanhTien = Math.Max(0, tienHang + phiVanChuyen - giamGia);


        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        int newOrderId;
        try
        {
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
                TrangThai = OrderStatus.ChoXuLy,
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

            foreach (var item in cart.CartItems)
                await _productService.TruTonKhoAsync(item.VariantId, item.SoLuong);

            // Trừ lượt dùng coupon 
            if (dto.CouponId.HasValue)
            {
                var coupon = await _dbContext.Coupons.FindAsync(dto.CouponId.Value);
                await _couponService.SuDungMaAsync(coupon!.MaCode);
            }

            var tienHangThucChi = Math.Max(0, tienHang - giamGia);
            var soDiemTich = (int)(tienHangThucChi / 1000);
            if (soDiemTich > 0)
            {
                _dbContext.PawPointsTransactions.Add(new PawPointsTransaction
                {
                    UserId = userId,
                    OrderId = order.OrderId,
                    SoDiem = soDiemTich,
                    Loai = "earn",
                    NgayGiaoDich = DateTime.UtcNow
                });
            }

            // Trừ điểm nếu khách chọn quy đổi PawPoints ngay lúc đặt hàng 
            if (dto.SoDiemMuonDoi is > 0)
            {
                _dbContext.PawPointsTransactions.Add(new PawPointsTransaction
                {
                    UserId = userId,
                    OrderId = order.OrderId,
                    SoDiem = -dto.SoDiemMuonDoi.Value,
                    Loai = "redeem",
                    NgayGiaoDich = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();

            // Xóa giỏ hàng 
            await _cartService.XoaSachGioHangAsync(userId);

            await _auditLogService.LogAsync(userId, "TAO_DON_HANG", "orders", order.OrderId,
                $"Đặt hàng thành công, thành tiền {thanhTien:N0}đ");

            await transaction.CommitAsync();
            newOrderId = order.OrderId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return await GetByIdAsync(userId, newOrderId) ?? throw new InvalidOperationException("Lỗi tạo đơn hàng.");
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

    public async Task<PagedResult<OrderDto>> GetAllAsync(OrderFilterRequestDto filter)
    {
        var query = _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            query = query.Where(o => o.TrangThai == filter.TrangThai);

        if (filter.UserId.HasValue)
            query = query.Where(o => o.UserId == filter.UserId.Value);

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

    public async Task<OrderDto?> GetByIdAdminAsync(int orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CancelAsync(int userId, int orderId, CancelOrderRequestDto dto)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Variant).ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

        if (!OrderStatus.ChoPhepKhachHuy.Contains(order.TrangThai))
            throw new InvalidOperationException("Đơn hàng đã giao vận, không thể tự hủy. Vui lòng liên hệ hỗ trợ để yêu cầu trả hàng.");

        order.TrangThai = OrderStatus.DaHuy;

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
        if (!OrderStatus.TatCa.Contains(trangThaiMoi))
            throw new InvalidOperationException($"Trạng thái '{trangThaiMoi}' không hợp lệ.");

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
        HoTenKhachHang = o.User?.HoTen,
        EmailKhachHang = o.User?.Email,
        OrderItems = o.OrderItems.Select(oi => new OrderItemDto
        {
            OrderItemId = oi.OrderItemId,
            VariantId = oi.VariantId,
            ProductId = oi.Variant.ProductId,
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
        order.TrangThai = OrderStatus.DaGiaoVan;
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

        if (order.TrangThai != OrderStatus.DaGiao)
            throw new InvalidOperationException("Chỉ có thể yêu cầu trả hàng đối với đơn đã giao thành công.");

        order.TrangThai = OrderStatus.ChoTraHang;
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

        if (order.TrangThai != OrderStatus.ChoTraHang)
            throw new InvalidOperationException("Đơn hàng không ở trạng thái chờ trả hàng.");

        if (dongY)
        {
            order.TrangThai = OrderStatus.DaTraHang;

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
            order.TrangThai = OrderStatus.DaGiao; // từ chối, trả đơn về trạng thái đã giao như cũ
        }

        await _dbContext.SaveChangesAsync();
        return MapToDto(order);
    }

    public async Task<bool> DaMuaVaNhanHangAsync(int userId, int productId)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId && o.TrangThai == OrderStatus.DaGiao)
            .SelectMany(o => o.OrderItems)
            .AnyAsync(oi => oi.Variant.ProductId == productId);
    }

    public async Task<decimal> TongTietKiemAsync(int userId)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId && o.TrangThai != OrderStatus.DaHuy)
            .SumAsync(o => (decimal?)o.GiamGia) ?? 0m;
    }
}