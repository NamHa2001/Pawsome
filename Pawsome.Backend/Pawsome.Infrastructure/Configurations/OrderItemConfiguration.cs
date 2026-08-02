using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", t =>
        {
            t.HasCheckConstraint("CK_order_items_soluong", "so_luong > 0");
            t.HasCheckConstraint("CK_order_items_dongia", "don_gia >= 0");
        });
        builder.HasKey(oi => oi.OrderItemId);

        builder.Property(oi => oi.OrderItemId).HasColumnName("order_item_id");
        builder.Property(oi => oi.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(oi => oi.VariantId).HasColumnName("variant_id").IsRequired();
        builder.Property(oi => oi.SoLuong).HasColumnName("so_luong").IsRequired();
        builder.Property(oi => oi.DonGia).HasColumnName("don_gia").HasPrecision(12, 2).IsRequired();

        builder.HasIndex(oi => oi.OrderId).HasDatabaseName("IX_order_items_order");
        builder.HasIndex(oi => oi.VariantId).HasDatabaseName("IX_order_items_variant");

        builder.HasOne(oi => oi.Order)
               .WithMany(o => o.OrderItems)
               .HasForeignKey(oi => oi.OrderId)
               .HasConstraintName("FK_order_items_orders")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(oi => oi.Variant)
               .WithMany(v => v.OrderItems)
               .HasForeignKey(oi => oi.VariantId)
               .HasConstraintName("FK_order_items_variants")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
