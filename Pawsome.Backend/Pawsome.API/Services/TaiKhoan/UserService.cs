using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.TaiKhoan;

public class UserService : IUserService
{
    private readonly PawsomeDbContext _dbContext;

    public UserService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        user.HoTen = dto.HoTen;
        user.SoDienThoai = dto.SoDienThoai;
        await _dbContext.SaveChangesAsync();

        return MapToDto(user);
    }

    private static UserProfileDto MapToDto(Domain.Entities.TaiKhoan.User user) => new()
    {
        UserId = user.UserId,
        Email = user.Email,
        HoTen = user.HoTen,
        SoDienThoai = user.SoDienThoai,
        DiemPawpoints = user.DiemPawpoints,
        Role = user.Role.TenVaiTro,
        NgayTao = user.NgayTao
    };
}