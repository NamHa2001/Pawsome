using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants", t =>
        {
            t.HasCheckConstraint("CK_variants_gia", "gia > 0");
            t.HasCheckConstraint("CK_variants_soluongton", "so_luong_ton >= 0");
        });
        builder.HasKey(v => v.VariantId);

        builder.Property(v => v.VariantId).HasColumnName("variant_id");
        builder.Property(v => v.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(v => v.TenBienThe).HasColumnName("ten_bien_the").HasMaxLength(100).IsRequired();
        builder.Property(v => v.Gia).HasColumnName("gia").HasPrecision(12, 2).IsRequired();
        builder.Property(v => v.SoLuongTon).HasColumnName("so_luong_ton").HasDefaultValue(0);
        builder.Property(v => v.Sku).HasColumnName("sku").HasMaxLength(50);
        builder.Property(v => v.DangKinhDoanh).HasColumnName("dang_kinh_doanh").HasDefaultValue(true);

        builder.HasIndex(v => v.ProductId).HasDatabaseName("IX_variants_product");
        // Filtered unique index: sku chỉ cần duy nhất khi ĐÃ có giá trị (bỏ qua NULL) - xem Database.sql
        builder.HasIndex(v => v.Sku)
               .IsUnique()
               .HasDatabaseName("UQ_product_variants_sku")
               .HasFilter("[sku] IS NOT NULL");

        builder.HasOne(v => v.Product)
               .WithMany(p => p.Variants)
               .HasForeignKey(v => v.ProductId)
               .HasConstraintName("FK_variants_products")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
