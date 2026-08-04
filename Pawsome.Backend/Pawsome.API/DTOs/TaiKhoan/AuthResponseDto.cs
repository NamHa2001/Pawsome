namespace Pawsome.API.DTOs.TaiKhoan;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string Role { get; set; } = null!;
}