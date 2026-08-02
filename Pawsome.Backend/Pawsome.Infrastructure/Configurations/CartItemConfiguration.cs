using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.GioHang;

namespace Pawsome.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items", t => t.HasCheckConstraint("CK_cart_items_soluong", "so_luong > 0"));
        builder.HasKey(ci => ci.CartItemId);

        builder.Property(ci => ci.CartItemId).HasColumnName("cart_item_id");
        builder.Property(ci => ci.CartId).HasColumnName("cart_id").IsRequired();
        builder.Property(ci => ci.VariantId).HasColumnName("variant_id").IsRequired();
        builder.Property(ci => ci.SoLuong).HasColumnName("so_luong").HasDefaultValue(1);

        builder.HasIndex(ci => new { ci.CartId, ci.VariantId }).IsUnique().HasDatabaseName("UQ_cart_items_cart_variant");
        builder.HasIndex(ci => ci.CartId).HasDatabaseName("IX_cart_items_cart");
        builder.HasIndex(ci => ci.VariantId).HasDatabaseName("IX_cart_items_variant");

        builder.HasOne(ci => ci.Cart)
               .WithMany(c => c.CartItems)
               .HasForeignKey(ci => ci.CartId)
               .HasConstraintName("FK_cart_items_carts")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Variant)
               .WithMany(v => v.CartItems)
               .HasForeignKey(ci => ci.VariantId)
               .HasConstraintName("FK_cart_items_variants")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
