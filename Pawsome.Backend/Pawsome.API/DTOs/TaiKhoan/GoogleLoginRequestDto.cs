using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class GoogleLoginRequestDto
{
    [Required(ErrorMessage = "ID token không được để trống")]
    public string IdToken { get; set; } = null!;
}