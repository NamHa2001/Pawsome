using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pawsome.Domain.Entities.BlogQuanTri;
using Pawsome.Domain.Entities.Common;
using Pawsome.Domain.Entities.DonHang;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Infrastructure;

public class PawsomeDbContext : DbContext
{
    public PawsomeDbContext(DbContextOptions<PawsomeDbContext> options) : base(options)
    {
    }

    // TaiKhoan
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();

    // SanPham
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewVote> ReviewVotes => Set<ReviewVote>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<ProductCondition> ProductConditions => Set<ProductCondition>();

    // GioHang
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<AutoOrder> AutoOrders => Set<AutoOrder>();
    public DbSet<PawVipPayment> PawVipPayments => Set<PawVipPayment>();

    // DonHang
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PawPointsTransaction> PawPointsTransactions => Set<PawPointsTransaction>();

    // BlogQuanTri
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    // Common
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PawsomeDbContext).Assembly);

        // Toàn hệ thống lưu thời gian bằng DateTime.UtcNow (chuẩn UTC), nhưng cột datetime2 của
        // SQL Server không lưu thông tin múi giờ nên EF Core đọc lại luôn ra Kind=Unspecified -
        // khi serialize JSON bị thiếu hậu tố "Z", frontend hiểu nhầm là giờ local nên hiển thị
        // chậm hơn giờ Việt Nam 7 tiếng. Ép lại Kind=Utc ngay khi đọc để JSON luôn có "Z", trình
        // duyệt tự quy đổi đúng sang giờ máy người dùng.
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        var utcConverterNullable = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(utcConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(utcConverterNullable);
            }
        }
    }
}
