namespace Pawsome.API.DTOs.SanPham;

public class ProductSuggestionDto
{
    public int ProductId { get; set; }
    public string Ten { get; set; } = null!;
    public string? AnhChinh { get; set; }
    public decimal? GiaTu { get; set; }
}
