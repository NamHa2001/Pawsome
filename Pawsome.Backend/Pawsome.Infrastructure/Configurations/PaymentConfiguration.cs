using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", t => t.HasCheckConstraint("CK_payments_sotien", "so_tien > 0"));
        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.PaymentId).HasColumnName("payment_id");
        builder.Property(p => p.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(p => p.PhuongThuc).HasColumnName("phuong_thuc").HasMaxLength(30).IsRequired();
        builder.Property(p => p.SoTien).HasColumnName("so_tien").HasPrecision(12, 2).IsRequired();
        builder.Property(p => p.TrangThai).HasColumnName("trang_thai").HasMaxLength(20).IsRequired();
        builder.Property(p => p.MaGiaoDich).HasColumnName("ma_giao_dich").HasMaxLength(100);
        builder.Property(p => p.NgayThanhToan).HasColumnName("ngay_thanh_toan");

        builder.HasIndex(p => p.OrderId).HasDatabaseName("IX_payments_order");

        builder.HasOne(p => p.Order)
               .WithMany(o => o.Payments)
               .HasForeignKey(p => p.OrderId)
               .HasConstraintName("FK_payments_orders")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
