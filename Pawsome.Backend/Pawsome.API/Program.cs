using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pawsome.API.Common;
using Pawsome.API.Common.AuditLog;
using Pawsome.API.Common.Auth;
using Pawsome.API.Common.Middleware;
using Pawsome.API.Services.BlogQuanTri;
using Pawsome.API.Services.DonHang;
using Pawsome.API.Services.GioHang;
using Pawsome.API.Services.SanPham;
using Pawsome.API.Services.TaiKhoan;
using Pawsome.Infrastructure;
using System.Text;
using Pawsome.API.Common.Email;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<PawsomeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── MVC + Swagger ──────────────────────────────────────────────────────────
// ConfigureApiBehaviorOptions: chuẩn hóa lỗi tự động của [Required]/[StringLength]... về
// đúng khung ApiResponse<T> (mục 5.2 Pawsome_KhungDuAn.md) - mặc định ASP.NET Core trả
// ValidationProblemDetails, khác cấu trúc { success, data, message } nên Angular interceptor
// chung không đọc được nếu không chuẩn hóa ở đây.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var loiDauTien = context.ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Dữ liệu không hợp lệ.";

            return new BadRequestObjectResult(ApiResponse<object>.Fail(loiDauTien));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pawsome API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Chỉ dán chuỗi token vào đây, KHÔNG cần gõ chữ 'Bearer' - Swagger tự thêm vào."
    };
    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// ── CORS (cho phép Angular dev server gọi API) ────────────────────────────
const string AngularDevCorsPolicy = "AngularDevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//HttpClient: dùng chung để gọi API bên thứ 3 (MoMo, VNPay, GHN...) ────
builder.Services.AddHttpClient();

// ── Auth (Custom Authentication + JWT, KHÔNG dùng ASP.NET Core Identity -
// schema mặc định của Identity không khớp bảng users/roles đã thiết kế) ──
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; //giữ nguyên tên claim gốc ("sub", "email"...), không đổi tên
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            // Phải khớp đúng loại claim mà JwtTokenGenerator.cs thực sự phát hành
            // (new Claim(ClaimTypes.Role, role) - URI dài, không phải chuỗi "role"
            // ngắn). Đặt sai giá trị này khiến [Authorize(Roles=...)] luôn trả 403
            // dù JWT hợp lệ và đúng vai trò, vì User.IsInRole tìm claim type khác
            // với claim thực sự có trong token.
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Sub
        };
    });
builder.Services.AddAuthorization();

// ── Common: dùng chung cho cả 5 Phần (mục 6 Pawsome_KhungDuAn.md) ─────────
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ── DI của từng Phần: mỗi Phần tự thêm AddScoped<...>() của mình vào đây ──
// (đúng mục 5.8 Pawsome_KhungDuAn.md - chỗ duy nhất mọi Phần đều phải đụng
// vào cùng 1 file; kéo code mới nhất trước khi thêm dòng của mình để tránh xung đột)
// Phần 1 - Tài khoản:
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
// Phần 2 - Sản phẩm:
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IConditionService, ConditionService>();
builder.Services.AddScoped<IRecaptchaService, RecaptchaService>();
// Phần 3 - Giỏ hàng:      
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAutoOrderService, AutoOrderService>();
builder.Services.AddScoped<IPawVipService, PawVipService>();
builder.Services.AddScoped<IPawVipPaymentService, PawVipPaymentService>();
// Phần 4 - Đơn hàng:       
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPawPointsService, PawPointsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// Phần 5 - Blog/Quản trị:
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization(); 
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // phục vụ file tĩnh trong wwwroot (VD ảnh bìa blog upload lên - IBlogService.LuuAnhAsync)

app.UseCors(AngularDevCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();