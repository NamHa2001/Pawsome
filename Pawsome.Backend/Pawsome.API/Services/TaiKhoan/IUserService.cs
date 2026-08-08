using Pawsome.API.DTOs.TaiKhoan;

namespace Pawsome.API.Services.TaiKhoan;

// Hồ sơ cá nhân
public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
}