using Microsoft.EntityFrameworkCore;
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

    // GioHang
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<AutoOrder> AutoOrders => Set<AutoOrder>();

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
    }
}
