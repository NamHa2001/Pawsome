using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.Infrastructure;
using AuditLogEntity = Pawsome.Domain.Entities.Common.AuditLog;

namespace Pawsome.API.Common.AuditLog;

public interface IAuditLogService
{
    Task LogAsync(int? userId, string hanhDong, string? doiTuong = null, int? doiTuongId = null, string? chiTiet = null, string? diaChiIp = null);

    // Dùng bởi trang quản trị "Nhật ký hệ thống" (SRS mục 6.4) - trước đây bảng audit_logs chỉ
    // được ghi, không nơi nào đọc lại.
    Task<PagedResult<AuditLogDto>> GetAllAsync(AuditLogFilterDto filter);
}

public class AuditLogService : IAuditLogService
{
    private readonly PawsomeDbContext _dbContext;

    public AuditLogService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(int? userId, string hanhDong, string? doiTuong = null, int? doiTuongId = null, string? chiTiet = null, string? diaChiIp = null)
    {
        _dbContext.AuditLogs.Add(new AuditLogEntity
        {
            UserId = userId,
            HanhDong = hanhDong,
            DoiTuong = doiTuong,
            DoiTuongId = doiTuongId,
            ChiTiet = chiTiet,
            DiaChiIp = diaChiIp,
            NgayTao = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResult<AuditLogDto>> GetAllAsync(AuditLogFilterDto filter)
    {
        var query = _dbContext.AuditLogs.Include(l => l.User).AsQueryable();

        if (filter.UserId.HasValue)
            query = query.Where(l => l.UserId == filter.UserId.Value);
        if (!string.IsNullOrWhiteSpace(filter.HanhDong))
            query = query.Where(l => l.HanhDong.Contains(filter.HanhDong));
        if (filter.TuNgay.HasValue)
            query = query.Where(l => l.NgayTao >= filter.TuNgay.Value);
        if (filter.DenNgay.HasValue)
            query = query.Where(l => l.NgayTao <= filter.DenNgay.Value);

        query = (IOrderedQueryable<AuditLogEntity>)query.OrderByDescending(l => l.NgayTao);

        var tongSo = await query.CountAsync();
        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 20 : Math.Min(filter.PageSize, 100);

        var items = await query.Skip((trang - 1) * soDong).Take(soDong)
            .Select(l => new AuditLogDto
            {
                LogId = l.LogId,
                UserId = l.UserId,
                HoTenNguoiDung = l.User != null ? l.User.HoTen : null,
                HanhDong = l.HanhDong,
                DoiTuong = l.DoiTuong,
                DoiTuongId = l.DoiTuongId,
                ChiTiet = l.ChiTiet,
                DiaChiIp = l.DiaChiIp,
                NgayTao = l.NgayTao
            })
            .ToListAsync();

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }
}
