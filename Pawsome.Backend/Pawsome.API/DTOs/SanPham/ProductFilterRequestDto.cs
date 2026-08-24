namespace Pawsome.API.DTOs.SanPham;

public class ProductFilterRequestDto
{
    public string? TuKhoa { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? GiaMin { get; set; }
    public decimal? GiaMax { get; set; }
    public decimal? DanhGiaMin { get; set; }
    public int? ConditionId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
