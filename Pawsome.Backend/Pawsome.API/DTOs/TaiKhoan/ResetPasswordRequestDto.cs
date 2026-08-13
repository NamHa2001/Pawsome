using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class ResetPasswordRequestDto
{
    [Required(ErrorMessage = "Token không được để trống")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Mã OTP không được để trống")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải gồm 6 chữ số")]
    public string Otp { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = null!;
}