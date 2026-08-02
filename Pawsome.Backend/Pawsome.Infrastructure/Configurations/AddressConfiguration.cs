using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Infrastructure.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");
        builder.HasKey(a => a.AddressId);

        builder.Property(a => a.AddressId).HasColumnName("address_id");
        builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(a => a.NguoiNhan).HasColumnName("nguoi_nhan").HasMaxLength(100).IsRequired();
        builder.Property(a => a.SoDienThoai).HasColumnName("so_dien_thoai").HasMaxLength(20).IsRequired();
        builder.Property(a => a.DiaChiChiTiet).HasColumnName("dia_chi_chi_tiet").HasMaxLength(255).IsRequired();
        builder.Property(a => a.PhuongXa).HasColumnName("phuong_xa").HasMaxLength(100);
        builder.Property(a => a.QuanHuyen).HasColumnName("quan_huyen").HasMaxLength(100);
        builder.Property(a => a.TinhThanh).HasColumnName("tinh_thanh").HasMaxLength(100).IsRequired();
        builder.Property(a => a.LaMacDinh).HasColumnName("la_mac_dinh").HasDefaultValue(false);

        builder.HasIndex(a => a.UserId).HasDatabaseName("IX_addresses_user");

        builder.HasOne(a => a.User)
               .WithMany(u => u.Addresses)
               .HasForeignKey(a => a.UserId)
               .HasConstraintName("FK_addresses_users")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
