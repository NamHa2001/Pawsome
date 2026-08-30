using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.API.Services.GioHang;
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

        return MapToProfileDto(user);
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

        return MapToProfileDto(user);
    }

    //Admin

    public async Task<PagedResult<AdminUserDto>> GetAllUsersAsync(
        int pageNumber, int pageSize, string? keyword, string? trangThai)
    {
        var query = _dbContext.Users.Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(u => u.Email.Contains(k) || u.HoTen.Contains(k));
        }

        if (!string.IsNullOrWhiteSpace(trangThai))
            query = query.Where(u => u.TrangThai == trangThai);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(u => u.NgayTao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => MapToAdminDto(u))
            .ToListAsync();

        return new PagedResult<AdminUserDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task SetLockStatusAsync(int userId, bool locked)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        user.TrangThai = locked ? "locked" : "active";
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeRoleAsync(int userId, int newRoleId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        var roleTonTai = await _dbContext.Roles.AnyAsync(r => r.RoleId == newRoleId);
        if (!roleTonTai)
            throw new InvalidOperationException("Vai trò không hợp lệ.");

        user.RoleId = newRoleId;
        await _dbContext.SaveChangesAsync();
    }

    private static UserProfileDto MapToProfileDto(Domain.Entities.TaiKhoan.User user) => new()
    {
        UserId = user.UserId,
        Email = user.Email,
        HoTen = user.HoTen,
        SoDienThoai = user.SoDienThoai,
        DiemPawpoints = user.DiemPawpoints,
        PawVipTier = PawVipTiers.ConHieuLuc(user.PawVipTier, user.PawVipHetHan, DateOnly.FromDateTime(DateTime.UtcNow))
            ? user.PawVipTier : null,
        PawVipHetHan = user.PawVipHetHan,
        Role = user.Role.TenVaiTro,
        NgayTao = user.NgayTao
    };

    private static AdminUserDto MapToAdminDto(Domain.Entities.TaiKhoan.User user) => new()
    {
        UserId = user.UserId,
        Email = user.Email,
        HoTen = user.HoTen,
        SoDienThoai = user.SoDienThoai,
        DiemPawpoints = user.DiemPawpoints,
        TrangThai = user.TrangThai,
        Role = user.Role.TenVaiTro,
        NgayTao = user.NgayTao
    };
}