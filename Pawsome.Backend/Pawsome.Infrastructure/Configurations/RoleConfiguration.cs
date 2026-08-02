using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId).HasColumnName("role_id");
        builder.Property(r => r.TenVaiTro).HasColumnName("ten_vai_tro").HasMaxLength(50).IsRequired();
        builder.Property(r => r.MoTa).HasColumnName("mo_ta").HasMaxLength(255);

        builder.HasIndex(r => r.TenVaiTro).IsUnique().HasDatabaseName("UQ_roles_ten");

        builder.HasData(
            new Role { RoleId = 1, TenVaiTro = "Customer", MoTa = "Khách hàng - người mua sản phẩm" },
            new Role { RoleId = 2, TenVaiTro = "Admin", MoTa = "Quản trị viên - quản lý toàn hệ thống" },
            new Role { RoleId = 3, TenVaiTro = "Moderator", MoTa = "Người kiểm duyệt - duyệt đánh giá, nội dung" },
            new Role { RoleId = 4, TenVaiTro = "Support", MoTa = "Nhân viên hỗ trợ - chăm sóc khách hàng" }
        );
    }
}
