using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.Domain.Entities.BlogQuanTri;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.BlogQuanTri;

public class WishlistService : IWishlistService
{
    private readonly PawsomeDbContext _dbContext;

    public WishlistService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<WishlistItemDto>> GetByUserAsync(int userId, int page, int pageSize)
    {
        var query = _dbContext.Wishlists
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.NgayThem);

        var tongSo = await query.CountAsync();

        var trang = page < 1 ? 1 : page;
        var soDong = pageSize < 1 ? 20 : Math.Min(pageSize, 100);

        // Kẹp "trang" về giá trị hợp lệ trước Skip/Take - tránh (trang - 1) * soDong tràn số
        // nguyên khi client gửi page rất lớn (VD 2000000000, vẫn là int hợp lệ nên qua được
        // model binding), giống lỗi đã phát hiện và sửa ở BlogService.SearchAsync.
        var tongSoTrang = PhanTrangHelper.TinhTongSoTrang(tongSo, soDong);
        trang = tongSoTrang > 0 ? Math.Min(trang, tongSoTrang) : 1;

        var items = await query
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .ToListAsync();

        return new PagedResult<WishlistItemDto>
        {
            Items = items.Select(w => MapToDto(w)).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    public async Task<WishlistItemDto> AddAsync(int userId, int productId)
    {
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            throw new InvalidOperationException("Sản phẩm không tồn tại.");

        var daTonTai = await _dbContext.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        if (daTonTai)
            throw new InvalidOperationException("Sản phẩm đã có trong danh sách yêu thích.");

        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId,
            NgayThem = DateTime.UtcNow
        };

        _dbContext.Wishlists.Add(wishlist);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // AnyAsync ở trên chỉ là pre-check (đọc trước khi ghi) nên vẫn có race: 2 request thêm
            // cùng sản phẩm gửi gần như đồng thời (double-click, mở 2 tab) có thể cùng vượt qua
            // pre-check trước khi request nào SaveChanges, request lưu sau sẽ vi phạm
            // UQ_wishlists_user_product thật của DB - bắt lỗi đó ở đây để trả về đúng thông báo
            // nghiệp vụ thay vì để lộ ra thành lỗi 500, giống cách ReviewService.BoPhieuAsync đã làm.
            throw new InvalidOperationException("Sản phẩm đã có trong danh sách yêu thích.");
        }

        return MapToDto(wishlist, product);
    }

    public async Task RemoveAsync(int userId, int productId)
    {
        var wishlist = await _dbContext.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (wishlist == null)
            throw new KeyNotFoundException("Sản phẩm không có trong danh sách yêu thích.");

        _dbContext.Wishlists.Remove(wishlist);
        await _dbContext.SaveChangesAsync();
    }

    private static WishlistItemDto MapToDto(Wishlist w, Domain.Entities.SanPham.Product? productGhiDe = null)
    {
        var product = productGhiDe ?? w.Product;
        return new WishlistItemDto
        {
            WishlistId = w.WishlistId,
            ProductId = w.ProductId,
            TenSanPham = product.Ten,
            AnhChinh = product.Images.FirstOrDefault(i => i.LaAnhChinh)?.Url ?? product.Images.FirstOrDefault()?.Url,
            GiaTu = product.GiaTu,
            DangKinhDoanh = product.DangKinhDoanh,
            NgayThem = w.NgayThem
        };
    }
}
