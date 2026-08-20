using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.Domain.Entities.BlogQuanTri;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.BlogQuanTri;

public class BlogService : IBlogService
{
    private readonly PawsomeDbContext _dbContext;

    public BlogService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<BlogPostDto>> SearchAsync(BlogFilterRequestDto filter)
    {
        var query = _dbContext.BlogPosts
            .Include(b => b.TacGia)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var tuKhoaSach = filter.TuKhoa.Trim();
            query = query.Where(b => b.TieuDe.Contains(tuKhoaSach) || b.NoiDung.Contains(tuKhoaSach));
        }

        if (!string.IsNullOrWhiteSpace(filter.ChuDe))
            query = query.Where(b => b.ChuDe == filter.ChuDe.Trim());

        var tongSo = await query.CountAsync();

        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 20 : Math.Min(filter.PageSize, 100);

        // Kẹp về trang cuối hợp lệ ngay tại đây (VD URL cũ/bộ lọc mới làm giảm tổng số
        // trang) để trả đúng dữ liệu trong 1 lượt gọi - tránh Frontend phải phát hiện
        // rồi gọi lại lần 2 (nhấp nháy loading không cần thiết).
        var tongSoTrang = PhanTrangHelper.TinhTongSoTrang(tongSo, soDong);
        // Khi không có kết quả nào (tongSoTrang = 0), luôn đưa trang về 1 - không được bỏ
        // qua bước này, vì "trang" có thể là giá trị rất lớn từ client (VD Int32.MaxValue)
        // và (trang - 1) * soDong sẽ tràn số nguyên (int overflow) nếu không kẹp, khiến EF
        // Core dịch ra OFFSET âm và SQL Server ném lỗi 500 thay vì trả danh sách rỗng.
        trang = tongSoTrang > 0 ? Math.Min(trang, tongSoTrang) : 1;

        var items = await query
            .OrderByDescending(b => b.NgayDang)
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .ToListAsync();

        return new PagedResult<BlogPostDto>
        {
            Items = items.Select(b => MapToDto(b)).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    // Danh sách chủ đề hiện có để Frontend dựng bộ lọc "phân loại theo chủ đề" (YC-8.2) -
    // blog_posts.chu_de chỉ là cột NVARCHAR tự do, không có bảng chủ đề riêng (Database.sql).
    public async Task<List<string>> GetChuDeListAsync()
    {
        return await _dbContext.BlogPosts
            .Where(b => b.ChuDe != null)
            .Select(b => b.ChuDe!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id)
    {
        var post = await _dbContext.BlogPosts
            .Include(b => b.TacGia)
            .FirstOrDefaultAsync(b => b.PostId == id);

        return post == null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto> CreateAsync(int tacGiaId, BlogPostRequestDto dto)
    {
        var tacGia = await _dbContext.Users.FindAsync(tacGiaId);
        if (tacGia == null)
            throw new InvalidOperationException("Tài khoản tác giả không tồn tại.");

        var post = new BlogPost
        {
            TieuDe = dto.TieuDe,
            NoiDung = dto.NoiDung,
            ChuDe = ChuanHoaChuDe(dto.ChuDe),
            TacGiaId = tacGiaId,
            AnhDaiDien = dto.AnhDaiDien,
            NgayDang = DateTime.UtcNow
        };

        _dbContext.BlogPosts.Add(post);
        await _dbContext.SaveChangesAsync();

        return MapToDto(post, tacGia.HoTen);
    }

    public async Task<BlogPostDto> UpdateAsync(int id, BlogPostRequestDto dto)
    {
        var post = await _dbContext.BlogPosts
            .Include(b => b.TacGia)
            .FirstOrDefaultAsync(b => b.PostId == id);

        if (post == null)
            throw new KeyNotFoundException("Không tìm thấy bài viết.");

        post.TieuDe = dto.TieuDe;
        post.NoiDung = dto.NoiDung;
        post.ChuDe = ChuanHoaChuDe(dto.ChuDe);
        post.AnhDaiDien = dto.AnhDaiDien;

        await _dbContext.SaveChangesAsync();

        return MapToDto(post);
    }

    public async Task DeleteAsync(int id)
    {
        // blog_posts không có cột trạng thái/soft-delete trong Database.sql (khác products/users)
        // nên xóa cứng là đúng thiết kế.
        var post = await _dbContext.BlogPosts.FindAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Không tìm thấy bài viết.");

        _dbContext.BlogPosts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }

    // Chuẩn hóa lúc ghi để khớp với Trim() lúc lọc ở SearchAsync - nếu không, chu_de dư khoảng trắng
    // sẽ khiến GetChuDeListAsync trả về "topic bẩn" và lọc theo chu_de sạch bị trật (0 kết quả).
    private static string? ChuanHoaChuDe(string? chuDe) =>
        string.IsNullOrWhiteSpace(chuDe) ? null : chuDe.Trim();

    private static BlogPostDto MapToDto(BlogPost b, string? tenTacGiaGhiDe = null) => new()
    {
        PostId = b.PostId,
        TieuDe = b.TieuDe,
        NoiDung = b.NoiDung,
        ChuDe = b.ChuDe,
        TacGiaId = b.TacGiaId,
        TenTacGia = tenTacGiaGhiDe ?? b.TacGia?.HoTen ?? "",
        AnhDaiDien = b.AnhDaiDien,
        NgayDang = b.NgayDang
    };
}
