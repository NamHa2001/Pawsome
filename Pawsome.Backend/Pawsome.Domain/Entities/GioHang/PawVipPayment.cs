using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.GioHang;

public class PawVipPayment
{
    public int PawVipPaymentId { get; set; }
    public int UserId { get; set; }
    public string Tier { get; set; } = null!;
    public decimal SoTien { get; set; }
    public string PhuongThuc { get; set; } = null!;
    public string TrangThai { get; set; } = null!;
    public string? MaGiaoDich { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime? NgayThanhToan { get; set; }

    public User User { get; set; } = null!;
}
