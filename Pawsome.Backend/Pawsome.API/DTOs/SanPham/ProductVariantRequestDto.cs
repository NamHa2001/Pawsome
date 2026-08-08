using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ProductVariantRequestDto
{
    [Required(ErrorMessage = "Tên biến thể không được để trống", AllowEmptyStrings = false)]
    [StringLength(100, ErrorMessage = "Tên biến thể không được vượt quá 100 ký tự")]
    public string TenBienThe { get; set; } = null!;

    [Range(0.01, double.MaxValue, ErrorMessage = "Giá biến thể phải lớn hơn 0")]
    public decimal Gia { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
    public int SoLuongTon { get; set; }

    [StringLength(50, ErrorMessage = "SKU không được vượt quá 50 ký tự")]
    public string? Sku { get; set; }
}
