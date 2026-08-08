using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ProductImageRequestDto
{
    [Required(ErrorMessage = "Đường dẫn ảnh không được để trống", AllowEmptyStrings = false)]
    [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
    public string Url { get; set; } = null!;

    public bool LaAnhChinh { get; set; }
}
