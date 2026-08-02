using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.Common;

public class AuditLog
{
    public int LogId { get; set; }
    public int? UserId { get; set; }
    public string HanhDong { get; set; } = null!;
    public string? DoiTuong { get; set; }
    public int? DoiTuongId { get; set; }
    public string? ChiTiet { get; set; }
    public string? DiaChiIp { get; set; }
    public DateTime NgayTao { get; set; }

    public User? User { get; set; }
}
