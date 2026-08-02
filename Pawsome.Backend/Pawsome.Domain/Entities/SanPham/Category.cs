namespace Pawsome.Domain.Entities.SanPham;

public class Category
{
    public int CategoryId { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public int? DanhMucChaId { get; set; }
    public string? MoTa { get; set; }

    public Category? DanhMucCha { get; set; }
    public ICollection<Category> DanhMucCon { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
