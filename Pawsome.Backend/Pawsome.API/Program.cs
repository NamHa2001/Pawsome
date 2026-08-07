using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pawsome.API.Common.AuditLog;
using Pawsome.API.Common.Auth;
using Pawsome.API.Common.Middleware;
using Pawsome.API.Services.SanPham;
using Pawsome.API.Services.TaiKhoan;
using Pawsome.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<PawsomeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── MVC + Swagger ──────────────────────────────────────────────────────────
builder.Services.AddControllers();
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
        Description = "Nhập token dạng: Bearer {token}"
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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });
builder.Services.AddAuthorization();

// ── Common: dùng chung cho cả 5 Phần (mục 6 Pawsome_KhungDuAn.md) ─────────
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// ── DI của từng Phần: mỗi Phần tự thêm AddScoped<...>() của mình vào đây ──
// (đúng mục 5.8 Pawsome_KhungDuAn.md - chỗ duy nhất mọi Phần đều phải đụng
// vào cùng 1 file; kéo code mới nhất trước khi thêm dòng của mình để tránh xung đột)
// Phần 1 - Tài khoản:
builder.Services.AddScoped<IAuthService, AuthService>();
// Phần 2 - Sản phẩm:       builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
// Phần 3 - Giỏ hàng:       builder.Services.AddScoped<ICartService, CartService>();
// Phần 4 - Đơn hàng:       builder.Services.AddScoped<IOrderService, OrderService>();
// Phần 5 - Blog/Quản trị:  builder.Services.AddScoped<IBlogService, BlogService>();

var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AngularDevCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
