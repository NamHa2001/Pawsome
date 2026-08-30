using Pawsome.API.Common;
using Pawsome.API.DTOs.TaiKhoan;

namespace Pawsome.API.Services.TaiKhoan;

// Hồ sơ cá nhân
public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);

    Task<PagedResult<AdminUserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? keyword, string? trangThai);
    Task<AdminUserDto?> GetUserByIdAdminAsync(int userId);
    Task SetLockStatusAsync(int userId, bool locked);
    Task ChangeRoleAsync(int userId, int newRoleId);
}