using Pawsome.Infrastructure;
using AuditLogEntity = Pawsome.Domain.Entities.Common.AuditLog;

namespace Pawsome.API.Common.AuditLog;

public interface IAuditLogService
{
    Task LogAsync(int? userId, string hanhDong, string? doiTuong = null, int? doiTuongId = null, string? chiTiet = null, string? diaChiIp = null);
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
}
