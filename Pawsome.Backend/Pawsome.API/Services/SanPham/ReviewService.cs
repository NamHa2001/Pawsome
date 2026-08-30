using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.DonHang;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class ReviewService : IReviewService
{
    private readonly PawsomeDbContext _dbContext;
    private readonly IOrderService _orderService;

    public ReviewService(PawsomeDbContext dbContext, IOrderService orderService)
    {
        _dbContext = dbContext;
        _orderService = orderService;
    }

    public Task<bool> CoTheDanhGiaAsync(int userId, int productId) =>
        _orderService.DaMuaVaNhanHangAsync(userId, productId);

    public async Task<PagedResult<ReviewDto>> GetByProductAsync(int productId, int page, int pageSize, int? currentUserId = null)
    {
        var query = _dbContext.Reviews
            .Include(r => r.User)
            .Include(r => r.Votes)
            .Where(r => r.ProductId == productId && r.TrangThai == "da_duyet")
            .OrderByDescending(r => r.NgayTao);

        var tongSo = await query.CountAsync();

        var trang = page < 1 ? 1 : page;
        var soDong = pageSize < 1 ? 20 : Math.Min(pageSize, 100);

        var items = await query
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .ToListAsync();

        return new PagedResult<ReviewDto>
        {
            Items = items.Select(r => MapToDto(r, currentUserId: currentUserId)).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    public async Task<List<ReviewDto>> GetNoiBatAsync(int soLuong)
    {
        var gioiHan = soLuong < 1 ? 6 : Math.Min(soLuong, 20);

        var items = await _dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Votes)
            .Include(r => r.Product).ThenInclude(p => p.Images)
            .Where(r => r.TrangThai == "da_duyet" && r.BinhLuan != null && r.BinhLuan != "")
            .OrderByDescending(r => r.SoSao)
            .ThenByDescending(r => r.NgayTao)
            .Take(gioiHan)
            .ToListAsync();

        return items.Select(r =>
        {
            var dto = MapToDto(r);
            dto.TenSanPham = r.Product.Ten;
            dto.AnhSanPham = r.Product.Images.FirstOrDefault(i => i.LaAnhChinh)?.Url
                ?? r.Product.Images.FirstOrDefault()?.Url;
            dto.DiemDanhGiaTbSanPham = r.Product.DiemDanhGiaTb;
            return dto;
        }).ToList();
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

        // Verified purchase - tự kiểm tra lại ở đây, không chỉ tin frontend đã ẩn form đúng
        // (CoTheDanhGiaAsync), phòng khách gọi thẳng API bỏ qua giao diện.
        var duocPhepDanhGia = await CoTheDanhGiaAsync(userId, dto.ProductId);
        if (!duocPhepDanhGia)
            throw new InvalidOperationException("Bạn cần mua và nhận sản phẩm này trước khi đánh giá.");

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            SoSao = dto.SoSao,
            BinhLuan = dto.BinhLuan,
            DiemChatLuong = dto.DiemChatLuong,
            DiemGiaTri = dto.DiemGiaTri,
            DiemHaiLongThuCung = dto.DiemHaiLongThuCung,
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

    // Vote hữu ích/không hữu ích - chỉ đánh giá đã duyệt mới vote được (chỉ đánh giá này mới hiện
    // ra cho khách xem), và không tự vote cho đánh giá của chính mình.
    public async Task<ReviewDto> VoteAsync(int userId, int reviewId, bool huuIch)
    {
        var review = await _dbContext.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        if (review == null)
            throw new KeyNotFoundException("Không tìm thấy đánh giá.");

        if (review.TrangThai != "da_duyet")
            throw new InvalidOperationException("Chỉ có thể đánh giá tính hữu ích cho các đánh giá đã được duyệt.");

        if (review.UserId == userId)
            throw new InvalidOperationException("Không thể tự đánh giá tính hữu ích cho đánh giá của chính mình.");

        var phieuHienTai = await _dbContext.ReviewVotes
            .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == userId);

        if (phieuHienTai == null)
        {
            _dbContext.ReviewVotes.Add(new ReviewVote
            {
                ReviewId = reviewId,
                UserId = userId,
                HuuIch = huuIch,
                NgayTao = DateTime.UtcNow
            });
        }
        else if (phieuHienTai.HuuIch == huuIch)
        {
            // Bấm lại đúng lựa chọn cũ - gỡ vote (toggle off)
            _dbContext.ReviewVotes.Remove(phieuHienTai);
        }
        else
        {
            phieuHienTai.HuuIch = huuIch;
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // 2 request gần như đồng thời (double-click nhanh, hoặc mở 2 tab) cùng đọc thấy chưa
            // có phiếu rồi cùng INSERT - UQ_review_votes_review_user chặn dòng thứ 2, báo lỗi thân
            // thiện để khách bấm lại thay vì lộ lỗi hệ thống thô.
            throw new InvalidOperationException("Thao tác đang được xử lý, vui lòng thử lại.");
        }

        var dto = MapToDto(review);
        dto.SoPhieuHuuIch = await _dbContext.ReviewVotes.CountAsync(v => v.ReviewId == reviewId && v.HuuIch);
        dto.SoPhieuKhongHuuIch = await _dbContext.ReviewVotes.CountAsync(v => v.ReviewId == reviewId && !v.HuuIch);
        dto.PhieuCuaToi = await _dbContext.ReviewVotes
            .Where(v => v.ReviewId == reviewId && v.UserId == userId)
            .Select(v => (bool?)v.HuuIch)
            .FirstOrDefaultAsync();

        return dto;
    }

    private static ReviewDto MapToDto(Review r, string? tenNguoiDanhGiaGhiDe = null, int? currentUserId = null) => new()
    {
        ReviewId = r.ReviewId,
        ProductId = r.ProductId,
        UserId = r.UserId,
        TenNguoiDanhGia = tenNguoiDanhGiaGhiDe ?? r.User?.HoTen ?? "",
        SoSao = r.SoSao,
        BinhLuan = r.BinhLuan,
        TrangThai = r.TrangThai,
        NgayTao = r.NgayTao,
        DiemChatLuong = r.DiemChatLuong,
        DiemGiaTri = r.DiemGiaTri,
        DiemHaiLongThuCung = r.DiemHaiLongThuCung,
        SoPhieuHuuIch = r.Votes.Count(v => v.HuuIch),
        SoPhieuKhongHuuIch = r.Votes.Count(v => !v.HuuIch),
        PhieuCuaToi = currentUserId.HasValue
            ? r.Votes.Where(v => v.UserId == currentUserId.Value).Select(v => (bool?)v.HuuIch).FirstOrDefault()
            : null
    };
}
