using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.SanPham;

public class Review
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public byte SoSao { get; set; }
    public string? BinhLuan { get; set; }
    public string TrangThai { get; set; } = "cho_duyet";
    public DateTime NgayTao { get; set; }

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}
