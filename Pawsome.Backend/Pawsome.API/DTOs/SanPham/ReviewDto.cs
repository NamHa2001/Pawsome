namespace Pawsome.API.DTOs.SanPham;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string TenNguoiDanhGia { get; set; } = null!;
    public byte SoSao { get; set; }
    public string? BinhLuan { get; set; }
    public string TrangThai { get; set; } = null!;
    public DateTime NgayTao { get; set; }
}
