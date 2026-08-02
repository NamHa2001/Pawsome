using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.GioHang;

namespace Pawsome.Infrastructure.Configurations;

public class AutoOrderConfiguration : IEntityTypeConfiguration<AutoOrder>
{
    public void Configure(EntityTypeBuilder<AutoOrder> builder)
    {
        builder.ToTable("auto_orders", t =>
        {
            t.HasCheckConstraint("CK_auto_orders_soluong", "so_luong > 0");
            t.HasCheckConstraint("CK_auto_orders_trangthai", "trang_thai IN (N'active', N'paused', N'cancelled')");
        });
        builder.HasKey(a => a.AutoOrderId);

        builder.Property(a => a.AutoOrderId).HasColumnName("auto_order_id");
        builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(a => a.VariantId).HasColumnName("variant_id").IsRequired();
        builder.Property(a => a.SoLuong).HasColumnName("so_luong").IsRequired();
        builder.Property(a => a.TanSuat).HasColumnName("tan_suat").HasMaxLength(20).IsRequired();
        builder.Property(a => a.NgayKeTiep).HasColumnName("ngay_ke_tiep").IsRequired();
        builder.Property(a => a.TrangThai).HasColumnName("trang_thai").HasMaxLength(20).HasDefaultValue("active");

        builder.HasIndex(a => a.UserId).HasDatabaseName("IX_auto_orders_user");
        builder.HasIndex(a => a.VariantId).HasDatabaseName("IX_auto_orders_variant");

        builder.HasOne(a => a.User)
               .WithMany(u => u.AutoOrders)
               .HasForeignKey(a => a.UserId)
               .HasConstraintName("FK_auto_orders_users")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.Variant)
               .WithMany(v => v.AutoOrders)
               .HasForeignKey(a => a.VariantId)
               .HasConstraintName("FK_auto_orders_variants")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
