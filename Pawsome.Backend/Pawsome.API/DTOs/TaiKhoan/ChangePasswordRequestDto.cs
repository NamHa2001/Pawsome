using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    public string MatKhauCu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
    public string MatKhauMoi { get; set; } = string.Empty;
}