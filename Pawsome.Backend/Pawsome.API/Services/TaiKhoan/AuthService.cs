using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common.Auth;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.Domain.Entities.TaiKhoan;
using Pawsome.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using Pawsome.API.Common.Email;
using Pawsome.API.Services.DonHang;

namespace Pawsome.API.Services.TaiKhoan;

public class AuthService : IAuthService
{
    private const int CustomerRoleId = 1; // seed cố định: 1=Customer (xem migration)

    private readonly PawsomeDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;
    private readonly string _googleClientId;
    private readonly IEmailService _emailService;
    private readonly IPawPointsService _pawPointsService;

    public AuthService(PawsomeDbContext dbContext,IPasswordHasher passwordHasher,IJwtTokenGenerator jwtTokenGenerator,IConfiguration configuration,IEmailService emailService,IPawPointsService pawPointsService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
        _googleClientId = configuration["GoogleAuth:ClientId"]!;
        _emailService = emailService;
        _pawPointsService = pawPointsService;
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

        await _pawPointsService.CongDiemThuongDangKyAsync(user.UserId);

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

        var otp = Random.Shared.Next(0, 1000000).ToString("D6");
        var otpHash = _passwordHasher.HashPassword(otp);

        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim("purpose", "password_reset"),
            new Claim("otpHash", otpHash),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        var resetToken = new JwtSecurityTokenHandler().WriteToken(token);

        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendAsync(
                    user.Email,
                    "Mã xác thực đặt lại mật khẩu Pawsome",
                    $"Mã OTP của bạn là: {otp}\n\nMã có hiệu lực trong 10 phút. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email OTP: {ex.Message}");
            }
        });

        return resetToken;
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
            throw new InvalidOperationException("Yêu cầu đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
        }

        var purpose = principal.FindFirstValue("purpose");
        var otpHash = principal.FindFirstValue("otpHash");
        if (purpose != "password_reset" || otpHash == null)
            throw new InvalidOperationException("Yêu cầu đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

        if (!_passwordHasher.VerifyPassword(dto.Otp, otpHash))
            throw new InvalidOperationException("Mã OTP không đúng.");

        var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Yêu cầu đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto dto)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleClientId }
            });
        }
        catch (InvalidJwtException)
        {
            throw new InvalidOperationException("Token Google không hợp lệ.");
        }

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                RoleId = CustomerRoleId,
                Email = payload.Email,
                PasswordHash = _passwordHasher.HashPassword(Guid.NewGuid().ToString()),
                HoTen = payload.Name ?? payload.Email,
                TrangThai = "active",
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Entry(user).Reference(u => u.Role).LoadAsync();
            await _pawPointsService.CongDiemThuongDangKyAsync(user.UserId);
        }

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
    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        if (!_passwordHasher.VerifyPassword(dto.MatKhauCu, user.PasswordHash))
            throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

        if (_passwordHasher.VerifyPassword(dto.MatKhauMoi, user.PasswordHash))
            throw new InvalidOperationException("Mật khẩu mới phải khác mật khẩu hiện tại.");

        user.PasswordHash = _passwordHasher.HashPassword(dto.MatKhauMoi);
        user.NgayCapNhat = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}