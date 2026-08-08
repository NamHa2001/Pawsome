using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class BrandRequestDto
{
    [Required(ErrorMessage = "Tên thương hiệu không được để trống", AllowEmptyStrings = false)]
    [StringLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự")]
    public string TenThuongHieu { get; set; } = null!;

    [StringLength(255, ErrorMessage = "Đường dẫn logo không được vượt quá 255 ký tự")]
    public string? LogoUrl { get; set; }
}
