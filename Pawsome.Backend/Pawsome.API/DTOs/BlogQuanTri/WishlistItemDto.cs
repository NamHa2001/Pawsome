namespace Pawsome.API.DTOs.BlogQuanTri;

public class WishlistItemDto
{
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public string TenSanPham { get; set; } = null!;
    public string? AnhChinh { get; set; }
    public decimal? GiaTu { get; set; }
    public bool DangKinhDoanh { get; set; }
    public DateTime NgayThem { get; set; }
}
