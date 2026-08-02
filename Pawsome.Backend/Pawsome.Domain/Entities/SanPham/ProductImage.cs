namespace Pawsome.Domain.Entities.SanPham;

public class ProductImage
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public string Url { get; set; } = null!;
    public bool LaAnhChinh { get; set; }

    public Product Product { get; set; } = null!;
}
