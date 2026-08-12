namespace Pawsome.API.DTOs.DonHang;

public class PawPointsTransactionDto
{
    public int TransactionId { get; set; }
    public int? OrderId { get; set; }
    public int SoDiem { get; set; }       // dương = cộng, âm = trừ/hoàn
    public string Loai { get; set; } = null!; // earn / redeem / bonus
    public DateTime NgayGiaoDich { get; set; }
}