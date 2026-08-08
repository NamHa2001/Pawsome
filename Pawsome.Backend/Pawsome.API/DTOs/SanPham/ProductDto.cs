namespace Pawsome.API.DTOs.SanPham;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Ten { get; set; } = null!;
    public string? MoTa { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string? LieuLuong { get; set; }
    public decimal? GiaTu { get; set; }
    public decimal DiemDanhGiaTb { get; set; }
    public int SoLuongDanhGia { get; set; }
    public bool DangKinhDoanh { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
}
