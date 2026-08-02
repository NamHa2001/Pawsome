using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(b => b.BrandId);

        builder.Property(b => b.BrandId).HasColumnName("brand_id");
        builder.Property(b => b.TenThuongHieu).HasColumnName("ten_thuong_hieu").HasMaxLength(100).IsRequired();
        builder.Property(b => b.LogoUrl).HasColumnName("logo_url").HasMaxLength(255);

        builder.HasIndex(b => b.TenThuongHieu).IsUnique().HasDatabaseName("UQ_brands_ten");
    }
}
