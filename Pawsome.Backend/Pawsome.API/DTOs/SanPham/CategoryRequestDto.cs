using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class CategoryRequestDto
{
    [Required(ErrorMessage = "Tên danh mục không được để trống", AllowEmptyStrings = false)]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
    public string TenDanhMuc { get; set; } = null!;

    public int? DanhMucChaId { get; set; }

    [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
    public string? MoTa { get; set; }
}