using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ConditionConfiguration : IEntityTypeConfiguration<Condition>
{
    public void Configure(EntityTypeBuilder<Condition> builder)
    {
        builder.ToTable("conditions");
        builder.HasKey(c => c.ConditionId);

        builder.Property(c => c.ConditionId).HasColumnName("condition_id");
        builder.Property(c => c.TenTinhTrang).HasColumnName("ten_tinh_trang").HasMaxLength(100).IsRequired();

        builder.HasIndex(c => c.TenTinhTrang).IsUnique().HasDatabaseName("UQ_conditions_ten");

        // Dữ liệu mẫu: đúng 6 mục đang hiển thị tĩnh ở menu "Shop by Condition"
        // (header.html) - không bịa thêm tình trạng nào ngoài UI đã có sẵn.
        builder.HasData(
            new Condition { ConditionId = 1, TenTinhTrang = "Flea, Tick" },
            new Condition { ConditionId = 2, TenTinhTrang = "Wormers" },
            new Condition { ConditionId = 3, TenTinhTrang = "Heartwormers" },
            new Condition { ConditionId = 4, TenTinhTrang = "Joint Care" },
            new Condition { ConditionId = 5, TenTinhTrang = "Skin & Coat" },
            new Condition { ConditionId = 6, TenTinhTrang = "Behavioural" }
        );
    }
}
