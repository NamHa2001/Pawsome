using Pawsome.API.DTOs.TaiKhoan;

namespace Pawsome.API.Services.TaiKhoan;

// Đăng ký / đăng nhập / quên - đặt lại mật khẩu / đăng nhập Google-Facebook
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
}
