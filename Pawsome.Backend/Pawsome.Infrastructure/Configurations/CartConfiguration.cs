using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.GioHang;

namespace Pawsome.Infrastructure.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");
        builder.HasKey(c => c.CartId);

        builder.Property(c => c.CartId).HasColumnName("cart_id");
        builder.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(c => c.CouponId).HasColumnName("coupon_id");
        builder.Property(c => c.NgayCapNhat).HasColumnName("ngay_cap_nhat").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(c => c.UserId).IsUnique().HasDatabaseName("UQ_carts_user");
        builder.HasIndex(c => c.CouponId).HasDatabaseName("IX_carts_coupon");

        builder.HasOne(c => c.User)
               .WithOne(u => u.Cart)
               .HasForeignKey<Cart>(c => c.UserId)
               .HasConstraintName("FK_carts_users")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Coupon)
               .WithMany()
               .HasForeignKey(c => c.CouponId)
               .HasConstraintName("FK_carts_coupons")
               .OnDelete(DeleteBehavior.SetNull);
    }
}
