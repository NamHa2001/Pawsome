using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", t =>
        {
            t.HasCheckConstraint("CK_reviews_sosao", "so_sao BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_reviews_trangthai", "trang_thai IN (N'cho_duyet', N'da_duyet', N'tu_choi')");
            t.HasCheckConstraint("CK_reviews_diemchatluong", "diem_chat_luong IS NULL OR diem_chat_luong BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_reviews_diemgiatri", "diem_gia_tri IS NULL OR diem_gia_tri BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_reviews_diemhailong", "diem_hai_long_thu_cung IS NULL OR diem_hai_long_thu_cung BETWEEN 1 AND 5");
            // Bảng reviews tự nó không có trigger, nhưng trigger trg_reviews_sync_avg (AFTER INSERT, UPDATE,
            // DELETE trên chính bảng reviews) vẫn khiến SQL Server chặn OUTPUT clause khi ghi vào bảng này.
            t.UseSqlOutputClause(false);
        });
        builder.HasKey(r => r.ReviewId);

        builder.Property(r => r.ReviewId).HasColumnName("review_id");
        builder.Property(r => r.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(r => r.SoSao).HasColumnName("so_sao").IsRequired();
        builder.Property(r => r.BinhLuan).HasColumnName("binh_luan");
        builder.Property(r => r.TrangThai).HasColumnName("trang_thai").HasMaxLength(20).HasDefaultValue("cho_duyet");
        builder.Property(r => r.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");
        builder.Property(r => r.DiemChatLuong).HasColumnName("diem_chat_luong");
        builder.Property(r => r.DiemGiaTri).HasColumnName("diem_gia_tri");
        builder.Property(r => r.DiemHaiLongThuCung).HasColumnName("diem_hai_long_thu_cung");

        builder.HasIndex(r => r.ProductId).HasDatabaseName("IX_reviews_product");
        builder.HasIndex(r => r.UserId).HasDatabaseName("IX_reviews_user");
        builder.HasIndex(r => r.TrangThai).HasDatabaseName("IX_reviews_trangthai");

        builder.HasOne(r => r.Product)
               .WithMany(p => p.Reviews)
               .HasForeignKey(r => r.ProductId)
               .HasConstraintName("FK_reviews_products")
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.User)
               .WithMany(u => u.Reviews)
               .HasForeignKey(r => r.UserId)
               .HasConstraintName("FK_reviews_users")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
