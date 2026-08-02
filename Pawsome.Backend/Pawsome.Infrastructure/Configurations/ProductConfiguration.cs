using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", t =>
        {
            t.HasCheckConstraint("CK_products_gia_tu", "gia_tu IS NULL OR gia_tu >= 0");
            t.HasCheckConstraint("CK_products_diem_danh_gia", "diem_danh_gia_tb BETWEEN 0 AND 5");
        });
        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.Ten).HasColumnName("ten").HasMaxLength(200).IsRequired();
        builder.Property(p => p.MoTa).HasColumnName("mo_ta");
        builder.Property(p => p.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(p => p.BrandId).HasColumnName("brand_id");
        builder.Property(p => p.LieuLuong).HasColumnName("lieu_luong").HasMaxLength(100);
        builder.Property(p => p.GiaTu).HasColumnName("gia_tu").HasPrecision(12, 2);
        builder.Property(p => p.DiemDanhGiaTb).HasColumnName("diem_danh_gia_tb").HasPrecision(2, 1).HasDefaultValue(0m);
        builder.Property(p => p.DangKinhDoanh).HasColumnName("dang_kinh_doanh").HasDefaultValue(true);
        builder.Property(p => p.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");
        builder.Property(p => p.NgayCapNhat).HasColumnName("ngay_cap_nhat").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_products_category");
        builder.HasIndex(p => p.BrandId).HasDatabaseName("IX_products_brand");

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .HasConstraintName("FK_products_categories")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.Brand)
               .WithMany(b => b.Products)
               .HasForeignKey(p => p.BrandId)
               .HasConstraintName("FK_products_brands")
               .OnDelete(DeleteBehavior.SetNull);
    }
}
