namespace Pawsome.API.Common.AuditLog;

public class AuditLogFilterDto
{
    public int? UserId { get; set; }
    public string? HanhDong { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
