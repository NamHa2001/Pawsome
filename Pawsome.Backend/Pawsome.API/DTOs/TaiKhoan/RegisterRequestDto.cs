using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }
}