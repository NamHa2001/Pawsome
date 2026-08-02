using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");
        builder.HasKey(i => i.ImageId);

        builder.Property(i => i.ImageId).HasColumnName("image_id");
        builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(i => i.Url).HasColumnName("url").HasMaxLength(255).IsRequired();
        builder.Property(i => i.LaAnhChinh).HasColumnName("la_anh_chinh").HasDefaultValue(false);

        builder.HasIndex(i => i.ProductId).HasDatabaseName("IX_images_product");

        builder.HasOne(i => i.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(i => i.ProductId)
               .HasConstraintName("FK_images_products")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
