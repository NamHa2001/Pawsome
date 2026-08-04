using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = null!;
}