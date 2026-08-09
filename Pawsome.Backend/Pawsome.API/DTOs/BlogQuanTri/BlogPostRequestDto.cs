using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.BlogQuanTri;

public class BlogPostRequestDto
{
    [Required(ErrorMessage = "Tiêu đề không được để trống", AllowEmptyStrings = false)]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string TieuDe { get; set; } = null!;

    [Required(ErrorMessage = "Nội dung không được để trống", AllowEmptyStrings = false)]
    public string NoiDung { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Chủ đề không được vượt quá 100 ký tự")]
    public string? ChuDe { get; set; }

    [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
    public string? AnhDaiDien { get; set; }
}
