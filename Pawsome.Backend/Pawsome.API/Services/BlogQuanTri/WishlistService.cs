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
        await _dbContext.SaveChangesAsync();

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
