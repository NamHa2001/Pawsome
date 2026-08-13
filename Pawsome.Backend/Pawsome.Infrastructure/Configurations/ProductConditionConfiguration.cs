using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ProductConditionConfiguration : IEntityTypeConfiguration<ProductCondition>
{
    public void Configure(EntityTypeBuilder<ProductCondition> builder)
    {
        builder.ToTable("product_conditions");
        builder.HasKey(pc => new { pc.ProductId, pc.ConditionId });

        builder.Property(pc => pc.ProductId).HasColumnName("product_id");
        builder.Property(pc => pc.ConditionId).HasColumnName("condition_id");

        // CASCADE cả 2 chiều: dòng liên kết không có ý nghĩa tồn tại độc lập
        // ngoài sản phẩm/tình trạng mà nó nối - cùng nguyên tắc đang dùng cho
        // product_images (xem ProductImageConfiguration).
        builder.HasOne(pc => pc.Product)
               .WithMany(p => p.ProductConditions)
               .HasForeignKey(pc => pc.ProductId)
               .HasConstraintName("FK_product_conditions_products")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Condition)
               .WithMany(c => c.ProductConditions)
               .HasForeignKey(pc => pc.ConditionId)
               .HasConstraintName("FK_product_conditions_conditions")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pc => pc.ConditionId).HasDatabaseName("IX_product_conditions_condition");
    }
}
