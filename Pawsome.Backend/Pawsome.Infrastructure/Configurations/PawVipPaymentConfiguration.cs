using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.GioHang;

namespace Pawsome.Infrastructure.Configurations;

public class PawVipPaymentConfiguration : IEntityTypeConfiguration<PawVipPayment>
{
    public void Configure(EntityTypeBuilder<PawVipPayment> builder)
    {
        builder.ToTable("pawvip_payments", t =>
        {
            t.HasCheckConstraint("CK_pawvip_payments_tier", "tier IN (N'thuong', N'nang-cao', N'vip')");
        });
        builder.HasKey(p => p.PawVipPaymentId);

        builder.Property(p => p.PawVipPaymentId).HasColumnName("pawvip_payment_id");
        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.Tier).HasColumnName("tier").HasMaxLength(20).IsRequired();
        builder.Property(p => p.SoTien).HasColumnName("so_tien").HasPrecision(12, 2).IsRequired();
        builder.Property(p => p.PhuongThuc).HasColumnName("phuong_thuc").HasMaxLength(20).IsRequired();
        builder.Property(p => p.TrangThai).HasColumnName("trang_thai").HasMaxLength(20).HasDefaultValue("cho_thanh_toan");
        builder.Property(p => p.MaGiaoDich).HasColumnName("ma_giao_dich").HasMaxLength(100);
        builder.Property(p => p.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");
        builder.Property(p => p.NgayThanhToan).HasColumnName("ngay_thanh_toan");

        builder.HasIndex(p => p.MaGiaoDich).IsUnique().HasDatabaseName("UQ_pawvip_payments_magiaodich");
        builder.HasIndex(p => p.UserId).HasDatabaseName("IX_pawvip_payments_user");

        builder.HasOne(p => p.User)
               .WithMany()
               .HasForeignKey(p => p.UserId)
               .HasConstraintName("FK_pawvip_payments_users")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
