namespace Pawsome.API.DTOs.TaiKhoan;

public class AdminUserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string? SoDienThoai { get; set; }
    public int DiemPawpoints { get; set; }
    public string? PawVipTier { get; set; }
    public DateOnly? PawVipHetHan { get; set; }
    public string TrangThai { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime NgayTao { get; set; }
}