using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", t =>
        {
            t.HasCheckConstraint("CK_users_trangthai", "trang_thai IN (N'active', N'locked')");
            t.HasCheckConstraint("CK_users_diem_pawpoints", "diem_pawpoints >= 0");
            t.HasCheckConstraint("CK_users_pawvip_tier", "pawvip_tier IS NULL OR pawvip_tier IN (N'thuong', N'nang-cao', N'vip')");
            // Bảng có trigger trg_users_set_ngaycapnhat (AFTER UPDATE) -> tắt OUTPUT clause của EF Core,
            // nếu không mọi UPDATE (vd cập nhật hồ sơ cá nhân) sẽ lỗi 500 do SQL Server chặn OUTPUT trên bảng có trigger.
            t.UseSqlOutputClause(false);
        });
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId).HasColumnName("user_id");
        builder.Property(u => u.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.HoTen).HasColumnName("ho_ten").HasMaxLength(100).IsRequired();
        builder.Property(u => u.SoDienThoai).HasColumnName("so_dien_thoai").HasMaxLength(20);
        builder.Property(u => u.DiemPawpoints).HasColumnName("diem_pawpoints").HasDefaultValue(0);
        builder.Property(u => u.PawVipTier).HasColumnName("pawvip_tier").HasMaxLength(20);
        builder.Property(u => u.PawVipHetHan).HasColumnName("pawvip_het_han");
        builder.Property(u => u.TrangThai).HasColumnName("trang_thai").HasMaxLength(20).HasDefaultValue("active");
        builder.Property(u => u.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");
        builder.Property(u => u.NgayCapNhat).HasColumnName("ngay_cap_nhat").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_users_email");
        builder.HasIndex(u => u.RoleId).HasDatabaseName("IX_users_role");

        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .HasConstraintName("FK_users_roles")
               .OnDelete(DeleteBehavior.NoAction);

        // Dữ liệu mẫu (Pawsome_PhanChia.docx - "Tầng 0: trưởng nhóm dựng toàn bộ bảng, dữ liệu mẫu"):
        // 1 tài khoản Admin để các Phần có sẵn quyền test API cần role Admin (vd đăng bài blog, CRUD sản phẩm).
        // Mật khẩu mẫu: Admin@123 (đã băm bằng BCrypt.Net-Next 4.0.3, cùng thư viện PasswordHasher đang dùng).
        builder.HasData(new User
        {
            UserId = 1,
            RoleId = 2,
            Email = "admin@pawsome.vn",
            PasswordHash = "$2a$11$VOB3KJqGRdIg7DYWqdKumuDJRPl4ED3pUwy36ArnXBg39m9Gz8wB.",
            HoTen = "Quản trị viên",
            SoDienThoai = null,
            DiemPawpoints = 0,
            TrangThai = "active",
            NgayTao = new DateTime(2026, 1, 1),
            NgayCapNhat = new DateTime(2026, 1, 1)
        });
    }
}
