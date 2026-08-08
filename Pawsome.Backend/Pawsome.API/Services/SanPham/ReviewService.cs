using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class ReviewService : IReviewService
{
    private readonly PawsomeDbContext _dbContext;

    public ReviewService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ReviewDto>> GetByProductAsync(int productId, int page, int pageSize)
    {
        var query = _dbContext.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.TrangThai == "da_duyet")
            .OrderByDescending(r => r.NgayTao);

        var tongSo = await query.CountAsync();

        var trang = page < 1 ? 1 : page;
        var soDong = pageSize < 1 ? 20 : pageSize;

        var items = await query
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .ToListAsync();

        return new PagedResult<ReviewDto>
        {
            Items = items.Select(r => MapToDto(r)).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    public async Task<List<ReviewDto>> GetChoDuyetAsync()
    {
        var items = await _dbContext.Reviews
            .Include(r => r.User)
            .Where(r => r.TrangThai == "cho_duyet")
            .OrderBy(r => r.NgayTao)
            .ToListAsync();

        return items.Select(r => MapToDto(r)).ToList();
    }

    public async Task<ReviewDto> CreateAsync(int userId, ReviewRequestDto dto)
    {
        var sanPhamTonTai = await _dbContext.Products.AnyAsync(p => p.ProductId == dto.ProductId);
        if (!sanPhamTonTai)
            throw new InvalidOperationException("Sản phẩm không tồn tại.");

        var nguoiDung = await _dbContext.Users.FindAsync(userId);
        if (nguoiDung == null)
            throw new InvalidOperationException("Tài khoản không tồn tại.");

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            SoSao = dto.SoSao,
            BinhLuan = dto.BinhLuan,
            TrangThai = "cho_duyet",
            NgayTao = DateTime.UtcNow
        };

        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync();

        return MapToDto(review, nguoiDung.HoTen);
    }

    public async Task<ReviewDto> DuyetAsync(int reviewId)
    {
        var review = await _dbContext.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        if (review == null)
            throw new KeyNotFoundException("Không tìm thấy đánh giá.");

        review.TrangThai = "da_duyet";
        await _dbContext.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task<ReviewDto> TuChoiAsync(int reviewId)
    {
        var review = await _dbContext.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        if (review == null)
            throw new KeyNotFoundException("Không tìm thấy đánh giá.");

        review.TrangThai = "tu_choi";
        await _dbContext.SaveChangesAsync();

        return MapToDto(review);
    }

    private static ReviewDto MapToDto(Review r, string? tenNguoiDanhGiaGhiDe = null) => new()
    {
        ReviewId = r.ReviewId,
        ProductId = r.ProductId,
        UserId = r.UserId,
        TenNguoiDanhGia = tenNguoiDanhGiaGhiDe ?? r.User?.HoTen ?? "",
        SoSao = r.SoSao,
        BinhLuan = r.BinhLuan,
        TrangThai = r.TrangThai,
        NgayTao = r.NgayTao
    };
}
