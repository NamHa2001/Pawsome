using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class UpdateProfileRequestDto
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }
}