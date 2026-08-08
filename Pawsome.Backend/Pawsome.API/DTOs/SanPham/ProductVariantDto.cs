namespace Pawsome.API.DTOs.SanPham;

public class ProductVariantDto
{
    public int VariantId { get; set; }
    public string TenBienThe { get; set; } = null!;
    public decimal Gia { get; set; }
    public int SoLuongTon { get; set; }
    public string? Sku { get; set; }
    public bool DangKinhDoanh { get; set; }
}
