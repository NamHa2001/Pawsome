namespace Pawsome.API.DTOs.SanPham;

public class CategoryRequestDto
{
    public string TenDanhMuc { get; set; } = null!;
    public int? DanhMucChaId { get; set; }
    public string? MoTa { get; set; }
}