using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ProductUpdateRequestDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống", AllowEmptyStrings = false)]
    [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
    public string Ten { get; set; } = null!;

    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Danh mục không được để trống")]
    public int CategoryId { get; set; }

    public int? BrandId { get; set; }

    [StringLength(100, ErrorMessage = "Liều lượng không được vượt quá 100 ký tự")]
    public string? LieuLuong { get; set; }
}
