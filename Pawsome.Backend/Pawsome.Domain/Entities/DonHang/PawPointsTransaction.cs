using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.DonHang;

public class PawPointsTransaction
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public int SoDiem { get; set; }
    public string Loai { get; set; } = null!;
    public DateTime NgayGiaoDich { get; set; }

    public User User { get; set; } = null!;
    public Order? Order { get; set; }
}
