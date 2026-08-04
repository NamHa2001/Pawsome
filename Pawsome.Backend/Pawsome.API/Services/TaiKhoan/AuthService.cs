using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common.Auth;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.Domain.Entities.TaiKhoan;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.TaiKhoan;

public class AuthService : IAuthService
{
    private const int CustomerRoleId = 1; // seed cố định: 1=Customer (xem migration)

    private readonly PawsomeDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        PawsomeDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            throw new InvalidOperationException("Email này đã được đăng ký.");

        var user = new User
        {
            RoleId = CustomerRoleId,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            HoTen = dto.HoTen,
            SoDienThoai = dto.SoDienThoai,
            TrangThai = "active",
            NgayTao = DateTime.UtcNow,
            NgayCapNhat = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(); // cần lưu trước để có UserId (identity)

        var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, "Customer");

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            Email = user.Email,
            HoTen = user.HoTen,
            Role = "Customer"
        };
    }
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new InvalidOperationException("Email hoặc mật khẩu không đúng.");

        if (user.TrangThai == "locked")
            throw new InvalidOperationException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");

        var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.Role.TenVaiTro);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            Email = user.Email,
            HoTen = user.HoTen,
            Role = user.Role.TenVaiTro
        };
    }
}