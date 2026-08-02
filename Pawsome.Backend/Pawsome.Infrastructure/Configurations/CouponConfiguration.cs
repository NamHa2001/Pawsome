using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.GioHang;

namespace Pawsome.Infrastructure.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons", t =>
        {
            t.HasCheckConstraint("CK_coupons_loai_giam", "loai_giam IN (N'percent', N'fixed')");
            t.HasCheckConstraint("CK_coupons_gia_tri", "gia_tri > 0");
            t.HasCheckConstraint("CK_coupons_so_luong", "so_luong IS NULL OR so_luong >= 0");
            t.HasCheckConstraint("CK_coupons_ngay", "ngay_bat_dau IS NULL OR ngay_ket_thuc IS NULL OR ngay_ket_thuc >= ngay_bat_dau");
        });
        builder.HasKey(c => c.CouponId);

        builder.Property(c => c.CouponId).HasColumnName("coupon_id");
        builder.Property(c => c.MaCode).HasColumnName("ma_code").HasMaxLength(50).IsRequired();
        builder.Property(c => c.LoaiGiam).HasColumnName("loai_giam").HasMaxLength(20).IsRequired();
        builder.Property(c => c.GiaTri).HasColumnName("gia_tri").HasPrecision(12, 2).IsRequired();
        builder.Property(c => c.NgayBatDau).HasColumnName("ngay_bat_dau");
        builder.Property(c => c.NgayKetThuc).HasColumnName("ngay_ket_thuc");
        builder.Property(c => c.SoLuong).HasColumnName("so_luong");

        builder.HasIndex(c => c.MaCode).IsUnique().HasDatabaseName("UQ_coupons_code");
    }
}
