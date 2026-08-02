using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId).HasColumnName("category_id");
        builder.Property(c => c.TenDanhMuc).HasColumnName("ten_danh_muc").HasMaxLength(100).IsRequired();
        builder.Property(c => c.DanhMucChaId).HasColumnName("danh_muc_cha_id");
        builder.Property(c => c.MoTa).HasColumnName("mo_ta").HasMaxLength(255);

        builder.HasIndex(c => c.DanhMucChaId).HasDatabaseName("IX_categories_parent");

        // FK tự tham chiếu: SQL Server không cho phép CASCADE/SET NULL trên FK tự tham chiếu -> NO ACTION
        builder.HasOne(c => c.DanhMucCha)
               .WithMany(c => c.DanhMucCon)
               .HasForeignKey(c => c.DanhMucChaId)
               .HasConstraintName("FK_categories_parent")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
