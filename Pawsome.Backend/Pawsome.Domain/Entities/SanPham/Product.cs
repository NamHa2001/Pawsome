using Pawsome.Domain.Entities.BlogQuanTri;

namespace Pawsome.Domain.Entities.SanPham;

public class Product
{
    public int ProductId { get; set; }
    public string Ten { get; set; } = null!;
    public string? MoTa { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string? LieuLuong { get; set; }
    public decimal? GiaTu { get; set; }
    public decimal DiemDanhGiaTb { get; set; }
    public bool DangKinhDoanh { get; set; } = true;
    public DateTime NgayTao { get; set; }
    public DateTime NgayCapNhat { get; set; }

    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
