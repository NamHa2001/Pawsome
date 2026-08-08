using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", t =>
        {
            t.HasCheckConstraint("CK_orders_tienhang", "tien_hang >= 0");
            t.HasCheckConstraint("CK_orders_phivanchuyen", "phi_van_chuyen >= 0");
            t.HasCheckConstraint("CK_orders_giamgia", "giam_gia >= 0");
            t.HasCheckConstraint("CK_orders_thanhtien", "thanh_tien >= 0");
            // Bảng có trigger trg_orders_set_ngaycapnhat (AFTER UPDATE) -> tắt OUTPUT clause của EF Core,
            // nếu không mọi UPDATE (vd cập nhật trạng thái đơn hàng) sẽ lỗi 500 do SQL Server chặn OUTPUT trên bảng có trigger.
            t.UseSqlOutputClause(false);
        });
        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.OrderId).HasColumnName("order_id");
        builder.Property(o => o.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(o => o.AddressId).HasColumnName("address_id").IsRequired();
        builder.Property(o => o.CouponId).HasColumnName("coupon_id");
        builder.Property(o => o.NgayDat).HasColumnName("ngay_dat").HasDefaultValueSql("GETDATE()");
        builder.Property(o => o.TienHang).HasColumnName("tien_hang").HasPrecision(12, 2).IsRequired();
        builder.Property(o => o.PhiVanChuyen).HasColumnName("phi_van_chuyen").HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(o => o.GiamGia).HasColumnName("giam_gia").HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(o => o.ThanhTien).HasColumnName("thanh_tien").HasPrecision(12, 2).IsRequired();
        builder.Property(o => o.TrangThai).HasColumnName("trang_thai").HasMaxLength(30).IsRequired();
        builder.Property(o => o.DonViVanChuyen).HasColumnName("don_vi_van_chuyen").HasMaxLength(30);
        builder.Property(o => o.MaVanDon).HasColumnName("ma_van_don").HasMaxLength(50);
        builder.Property(o => o.NgayCapNhat).HasColumnName("ngay_cap_nhat").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(o => o.UserId).HasDatabaseName("IX_orders_user");
        builder.HasIndex(o => o.AddressId).HasDatabaseName("IX_orders_address");
        builder.HasIndex(o => o.CouponId).HasDatabaseName("IX_orders_coupon");
        builder.HasIndex(o => o.TrangThai).HasDatabaseName("IX_orders_trangthai");
        builder.HasIndex(o => o.NgayDat).HasDatabaseName("IX_orders_ngaydat");

        builder.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .HasConstraintName("FK_orders_users")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Address)
               .WithMany(a => a.Orders)
               .HasForeignKey(o => o.AddressId)
               .HasConstraintName("FK_orders_addresses")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Coupon)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CouponId)
               .HasConstraintName("FK_orders_coupons")
               .OnDelete(DeleteBehavior.SetNull);
    }
}
