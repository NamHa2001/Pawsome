namespace Pawsome.API.DTOs.SanPham;

public class ProductImageDto
{
    public int ImageId { get; set; }
    public string Url { get; set; } = null!;
    public bool LaAnhChinh { get; set; }
}
