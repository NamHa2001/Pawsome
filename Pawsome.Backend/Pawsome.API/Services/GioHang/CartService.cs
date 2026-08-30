using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.Services.SanPham;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    public class CartService : ICartService
    {
        private readonly PawsomeDbContext _context;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartService(PawsomeDbContext context, IProductService productService, ICouponService couponService)
        {
            _context = context;
            _productService = productService;
            _couponService = couponService;
        }

        public async Task<CartDto> LayGioHangAsync(int userId)
        {
            var cart = await LayHoacTaoGioHangAsync(userId);
            return await MapToCartDtoAsync(cart);
        }

        public async Task<CartDto> ThemSanPhamAsync(int userId, AddCartItemDto dto)
        {
            var variant = await _productService.LayThongTinBienTheAsync(dto.VariantId)
                ?? throw new KeyNotFoundException("Biến thể sản phẩm không tồn tại");

            var cart = await LayHoacTaoGioHangAsync(userId);

            var item = cart.CartItems.FirstOrDefault(i => i.VariantId == dto.VariantId);
            if (item != null)
            { 
                item.SoLuong += dto.SoLuong;
            }
            else
            {
                item = new CartItem
                {
                    CartId = cart.CartId,
                    VariantId = dto.VariantId,
                    SoLuong = dto.SoLuong
                };
                // Chỉ cần add vào navigation collection - EF Core tự track vì cart đang là entity
                // được theo dõi (đã Include ở LayHoacTaoGioHangAsync). Trước đây add cả 2 nơi
                // (_context.CartItems.Add + cart.CartItems.Add) khiến cart.CartItems trong bộ nhớ
                // chứa 2 phần tử trỏ cùng 1 item, làm MapToCartDtoAsync bên dưới sinh ra 2
                // CartItemDto giống hệt nhau cho lần thêm sản phẩm mới đầu tiên (DB vẫn chỉ có
                // đúng 1 dòng, chỉ sai ở response ngay lúc đó).
                cart.CartItems.Add(item);
            }

            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return await MapToCartDtoAsync(cart);
        }

        public async Task<CartDto> CapNhatSoLuongAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var item = await LayCartItemCuaUserAsync(userId, cartItemId);
            item.SoLuong = dto.SoLuong;
            item.Cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return await MapToCartDtoAsync(item.Cart);
        }

        public async Task XoaSanPhamAsync(int userId, int cartItemId)
        {
            var item = await LayCartItemCuaUserAsync(userId, cartItemId);
            _context.CartItems.Remove(item);
            item.Cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task XoaSachGioHangAsync(int userId)
        {
            var cart = await LayHoacTaoGioHangAsync(userId);
            _context.CartItems.RemoveRange(cart.CartItems);
            cart.CouponId = null;
            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<ApplyCouponResultDto> ApDungMaGiamGiaAsync(int userId, ApplyCouponDto dto)
        {
            var cart = await LayHoacTaoGioHangAsync(userId);
            var cartDto = await MapToCartDtoAsync(cart);
            var coupon = await _couponService.KiemTraMaHopLeAsync(dto.MaCode);

            if (coupon == null)
            {
                return new ApplyCouponResultDto
                {
                    HopLe = false,
                    ThongBao = "Mã giảm giá không hợp lệ hoặc đã hết hạn",
                    TienHang = cartDto.TienHang,
                    GiamGia = 0,
                    PhiVanChuyenTamTinh = cartDto.PhiVanChuyenTamTinh,
                    TongTien = cartDto.TienHang + cartDto.PhiVanChuyenTamTinh
                };
            }

            // Lưu bền coupon đang áp lên cart - trước đây chỉ tính tạm rồi trả về, không ghi
            // gì vào DB, nên GET /api/gio-hang (kể cả khi load lại trang thanh toán) luôn mất
            // mã vừa áp. Xem CLAUDE.md / kế hoạch sửa bug coupon.
            cart.CouponId = coupon.CouponId;
            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            var giamGia = TinhGiamGia(coupon.LoaiGiam, coupon.GiaTri, cartDto.TienHang);

            return new ApplyCouponResultDto
            {
                HopLe = true,
                ThongBao = "Áp dụng mã giảm giá thành công",
                TienHang = cartDto.TienHang,
                GiamGia = giamGia,
                PhiVanChuyenTamTinh = cartDto.PhiVanChuyenTamTinh,
                TongTien = cartDto.TienHang - giamGia + cartDto.PhiVanChuyenTamTinh
            };
        }

        public async Task<CartDto> XoaMaGiamGiaAsync(int userId)
        {
            var cart = await LayHoacTaoGioHangAsync(userId);
            cart.CouponId = null;
            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return await MapToCartDtoAsync(cart);
        }

        private static decimal TinhGiamGia(string loaiGiam, decimal giaTri, decimal tienHang)
        {
            var giamGia = loaiGiam == "percent"
                ? Math.Round(tienHang * giaTri / 100, 0)
                : giaTri;

            return Math.Min(giamGia, tienHang); // không giảm quá tổng tiền hàng
        }
        private async Task<Cart> LayHoacTaoGioHangAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, NgayCapNhat = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private async Task<CartItem> LayCartItemCuaUserAsync(int userId, int cartItemId)
        {
            // Phải Include luôn CartItems của Cart (không chỉ Include(i => i.Cart)) - nếu không,
            // EF Core chỉ track đúng 1 CartItem này, và do quan hệ 2 chiều Cart<->CartItems, fixup
            // sẽ khiến cart.CartItems trong bộ nhớ chỉ chứa đúng 1 phần tử (chính nó), không phải
            // toàn bộ giỏ hàng. CapNhatSoLuongAsync gọi MapToCartDtoAsync(item.Cart) ngay sau đó sẽ
            // tính TienHang/GiamGia thiếu các dòng khác trong giỏ nếu giỏ có nhiều hơn 1 sản phẩm.
            var item = await _context.CartItems
                .Include(i => i.Cart).ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId && i.Cart.UserId == userId);

            return item ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ hàng");
        }

        private async Task<CartDto> MapToCartDtoAsync(Cart cart)
        {
            var dto = new CartDto
            {
                CartId = cart.CartId,
                PhiVanChuyenTamTinh = await TinhPhiVanChuyenTamTinhAsync(cart.UserId)
            };

            foreach (var item in cart.CartItems)
            {
                var variant = await _productService.LayThongTinBienTheAsync(item.VariantId);
                if (variant == null) continue;

                dto.Items.Add(new CartItemDto
                {
                    CartItemId = item.CartItemId,
                    VariantId = item.VariantId,
                    TenSanPham = variant.TenSanPham,
                    HinhAnh = variant.HinhAnhChinh,
                    ThuocTinh = variant.ThuocTinh,
                    DonGia = variant.Gia,
                    SoLuong = item.SoLuong
                });
            }

            if (cart.CouponId.HasValue)
            {
                await ApDungCouponDaLuuAsync(cart, dto);
            }

            return dto;
        }

        // Đọc coupon đã lưu bền trên cart.CouponId và tính lại giảm giá theo tổng tiền hàng
        // hiện tại. Nếu coupon đã hết hạn/hết lượt trong lúc nằm im ở giỏ (khách áp mã rồi để
        // đó vài ngày), tự gỡ khỏi cart luôn - không để CartDto trả về "đang áp dụng" một mã
        // đã hết hiệu lực.
        private async Task ApDungCouponDaLuuAsync(Cart cart, CartDto dto)
        {
            var coupon = await _context.Coupons.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CouponId == cart.CouponId);

            var homNay = DateOnly.FromDateTime(DateTime.Now);
            var conHieuLuc = coupon != null
                && (coupon.NgayBatDau == null || coupon.NgayBatDau <= homNay)
                && (coupon.NgayKetThuc == null || coupon.NgayKetThuc >= homNay)
                && (coupon.SoLuong == null || coupon.SoLuong > 0);

            if (!conHieuLuc)
            {
                cart.CouponId = null;
                await _context.SaveChangesAsync();
                return;
            }

            dto.CouponId = coupon!.CouponId;
            dto.MaCouponDangApDung = coupon.MaCode;
            dto.GiamGia = TinhGiamGia(coupon.LoaiGiam, coupon.GiaTri, dto.TienHang);
        }

        // Trước đây constant 30.000đ cho mọi khách, không kiểm tra gói PawVip - khách Advanced/VIP
        // vẫn bị tính phí dù đặc quyền miễn phí ship đã quảng cáo sẵn ở trang PawVip. Đây chỉ là
        // số TẠM TÍNH hiển thị ở giỏ hàng (chưa biết địa chỉ giao) - số thật theo tỉnh/thành
        // được OrderService tính lại khi tạo đơn, cũng áp cùng điều kiện miễn phí PawVip này.
        private async Task<decimal> TinhPhiVanChuyenTamTinhAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new { u.PawVipTier, u.PawVipHetHan })
                .FirstOrDefaultAsync();

            var homNay = DateOnly.FromDateTime(DateTime.Now);
            var mienPhiShip = user != null
                && PawVipTiers.ConHieuLuc(user.PawVipTier, user.PawVipHetHan, homNay)
                && PawVipTiers.TierMienPhiShip.Contains(user.PawVipTier!);

            return mienPhiShip ? 0m : 30000m;
        }
    }
}