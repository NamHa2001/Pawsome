namespace Pawsome.API.DTOs.SanPham;

public class BrandDto
{
    public int BrandId { get; set; }
    public string TenThuongHieu { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
