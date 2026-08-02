using Pawsome.Domain.Entities.SanPham;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.GioHang;

public class AutoOrder
{
    public int AutoOrderId { get; set; }
    public int UserId { get; set; }
    public int VariantId { get; set; }
    public int SoLuong { get; set; }
    public string TanSuat { get; set; } = null!;
    public DateOnly NgayKeTiep { get; set; }
    public string TrangThai { get; set; } = "active";

    public User User { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
