namespace Pawsome.Domain.Entities.SanPham;

public class Brand
{
    public int BrandId { get; set; }
    public string TenThuongHieu { get; set; } = null!;
    public string? LogoUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
