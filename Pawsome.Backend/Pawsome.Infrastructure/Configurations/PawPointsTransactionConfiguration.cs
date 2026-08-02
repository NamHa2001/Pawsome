using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Infrastructure.Configurations;

public class PawPointsTransactionConfiguration : IEntityTypeConfiguration<PawPointsTransaction>
{
    public void Configure(EntityTypeBuilder<PawPointsTransaction> builder)
    {
        builder.ToTable("pawpoints_transactions", t =>
        {
            t.HasCheckConstraint("CK_pawpoints_loai", "loai IN (N'earn', N'redeem', N'bonus')");
            t.HasCheckConstraint("CK_pawpoints_sodiem", "so_diem <> 0");
        });
        builder.HasKey(pt => pt.TransactionId);

        builder.Property(pt => pt.TransactionId).HasColumnName("transaction_id");
        builder.Property(pt => pt.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(pt => pt.OrderId).HasColumnName("order_id");
        builder.Property(pt => pt.SoDiem).HasColumnName("so_diem").IsRequired();
        builder.Property(pt => pt.Loai).HasColumnName("loai").HasMaxLength(20).IsRequired();
        builder.Property(pt => pt.NgayGiaoDich).HasColumnName("ngay_giao_dich").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(pt => pt.UserId).HasDatabaseName("IX_pawpoints_user");
        builder.HasIndex(pt => pt.OrderId).HasDatabaseName("IX_pawpoints_order");

        builder.HasOne(pt => pt.User)
               .WithMany(u => u.PawPointsTransactions)
               .HasForeignKey(pt => pt.UserId)
               .HasConstraintName("FK_pawpoints_users")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pt => pt.Order)
               .WithMany(o => o.PawPointsTransactions)
               .HasForeignKey(pt => pt.OrderId)
               .HasConstraintName("FK_pawpoints_orders")
               .OnDelete(DeleteBehavior.SetNull);
    }
}
