using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common.Auth;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.Domain.Entities.TaiKhoan;
using Pawsome.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Pawsome.API.Services.TaiKhoan;

public class AuthService : IAuthService
{
    private const int CustomerRoleId = 1; // seed cố định: 1=Customer (xem migration)

    private readonly PawsomeDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public AuthService(
        PawsomeDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
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
        await _dbContext.SaveChangesAsync();

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

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return string.Empty;

        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim("purpose", "password_reset"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear(); 

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(dto.Token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = key
            }, out _);
        }
        catch
        {
            throw new InvalidOperationException("Token không hợp lệ hoặc đã hết hạn.");
        }

        var purpose = principal.FindFirstValue("purpose");
        if (purpose != "password_reset")
            throw new InvalidOperationException("Token không hợp lệ hoặc đã hết hạn.");

        var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Token không hợp lệ hoặc đã hết hạn.");

        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        await _dbContext.SaveChangesAsync();
    }
}