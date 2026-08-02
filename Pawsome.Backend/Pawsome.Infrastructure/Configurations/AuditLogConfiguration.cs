using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.Common;

namespace Pawsome.Infrastructure.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(l => l.LogId);

        builder.Property(l => l.LogId).HasColumnName("log_id");
        builder.Property(l => l.UserId).HasColumnName("user_id");
        builder.Property(l => l.HanhDong).HasColumnName("hanh_dong").HasMaxLength(100).IsRequired();
        builder.Property(l => l.DoiTuong).HasColumnName("doi_tuong").HasMaxLength(50);
        builder.Property(l => l.DoiTuongId).HasColumnName("doi_tuong_id");
        builder.Property(l => l.ChiTiet).HasColumnName("chi_tiet");
        builder.Property(l => l.DiaChiIp).HasColumnName("dia_chi_ip").HasMaxLength(45);
        builder.Property(l => l.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(l => l.UserId).HasDatabaseName("IX_audit_logs_user");
        builder.HasIndex(l => l.NgayTao).HasDatabaseName("IX_audit_logs_ngaytao");

        builder.HasOne(l => l.User)
               .WithMany(u => u.AuditLogs)
               .HasForeignKey(l => l.UserId)
               .HasConstraintName("FK_audit_logs_users")
               .OnDelete(DeleteBehavior.SetNull);
    }
}
