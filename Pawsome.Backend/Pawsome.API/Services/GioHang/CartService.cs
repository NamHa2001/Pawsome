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
            return await MapToCartDtoAsync(cart, null);
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
                _context.CartItems.Add(item);
                cart.CartItems.Add(item);
            }

            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return await MapToCartDtoAsync(cart, null);
        }

        public async Task<CartDto> CapNhatSoLuongAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var item = await LayCartItemCuaUserAsync(userId, cartItemId);
            item.SoLuong = dto.SoLuong;
            item.Cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            return await MapToCartDtoAsync(item.Cart, null);
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
            cart.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<ApplyCouponResultDto> ApDungMaGiamGiaAsync(int userId, ApplyCouponDto dto)
        {
            var cart = await LayHoacTaoGioHangAsync(userId);
            var cartDto = await MapToCartDtoAsync(cart, null);
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

            var giamGia = coupon.LoaiGiam == "percent"
                ? Math.Round(cartDto.TienHang * coupon.GiaTri / 100, 0)
                : coupon.GiaTri;

            giamGia = Math.Min(giamGia, cartDto.TienHang); // không giảm quá tổng tiền hàng

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
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId && i.Cart.UserId == userId);

            return item ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ hàng");
        }

        private async Task<CartDto> MapToCartDtoAsync(Cart cart, string? maCoupon)
        {
            var dto = new CartDto
            {
                CartId = cart.CartId,
                MaCouponDangApDung = maCoupon,
                PhiVanChuyenTamTinh = TinhPhiVanChuyenTamTinh()
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

            return dto;
        }

        private static decimal TinhPhiVanChuyenTamTinh() => 30000m;
    }
}