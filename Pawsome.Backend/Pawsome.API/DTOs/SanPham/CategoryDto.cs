namespace Pawsome.API.DTOs.SanPham;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public int? DanhMucChaId { get; set; }
    public string? MoTa { get; set; }
}