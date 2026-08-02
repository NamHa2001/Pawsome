namespace Pawsome.Domain.Entities.DonHang;

public class Payment
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PhuongThuc { get; set; } = null!;
    public decimal SoTien { get; set; }
    public string TrangThai { get; set; } = null!;
    public string? MaGiaoDich { get; set; }
    public DateTime? NgayThanhToan { get; set; }

    public Order Order { get; set; } = null!;
}
