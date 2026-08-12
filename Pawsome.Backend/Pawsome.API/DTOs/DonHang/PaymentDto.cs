namespace Pawsome.API.DTOs.DonHang;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PhuongThuc { get; set; } = null!;
    public decimal SoTien { get; set; }
    public string TrangThai { get; set; } = null!;
    public string? MaGiaoDich { get; set; }
    public DateTime? NgayThanhToan { get; set; }
}