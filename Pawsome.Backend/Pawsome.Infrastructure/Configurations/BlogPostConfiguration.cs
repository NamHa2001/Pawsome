using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.BlogQuanTri;

namespace Pawsome.Infrastructure.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("blog_posts");
        builder.HasKey(b => b.PostId);

        builder.Property(b => b.PostId).HasColumnName("post_id");
        builder.Property(b => b.TieuDe).HasColumnName("tieu_de").HasMaxLength(200).IsRequired();
        builder.Property(b => b.NoiDung).HasColumnName("noi_dung").IsRequired();
        builder.Property(b => b.ChuDe).HasColumnName("chu_de").HasMaxLength(100);
        builder.Property(b => b.TacGiaId).HasColumnName("tac_gia_id").IsRequired();
        builder.Property(b => b.AnhDaiDien).HasColumnName("anh_dai_dien").HasMaxLength(255);
        builder.Property(b => b.NgayDang).HasColumnName("ngay_dang").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(b => b.TacGiaId).HasDatabaseName("IX_blog_posts_author");

        builder.HasOne(b => b.TacGia)
               .WithMany(u => u.BlogPosts)
               .HasForeignKey(b => b.TacGiaId)
               .HasConstraintName("FK_blog_posts_users")
               .OnDelete(DeleteBehavior.NoAction);

        // Dữ liệu mẫu (Pawsome_PhanChia.docx - Tầng 0), tác giả là user_id=1 (Admin mẫu ở UserConfiguration).
        builder.HasData(
            new BlogPost
            {
                PostId = 1,
                TieuDe = "5 lưu ý khi chăm sóc chó con mới về nhà",
                NoiDung = "Chó con mới tách mẹ cần thời gian thích nghi với môi trường mới. Hãy chuẩn bị chỗ ngủ ấm áp, " +
                          "lên lịch tiêm phòng đầy đủ, cho ăn đúng cữ với thức ăn dành riêng cho chó con, và tránh cho ra ngoài " +
                          "tiếp xúc với chó lạ trước khi hoàn thành các mũi tiêm cơ bản.",
                ChuDe = "Chó",
                TacGiaId = 1,
                AnhDaiDien = null,
                NgayDang = new DateTime(2026, 1, 2)
            },
            new BlogPost
            {
                PostId = 2,
                TieuDe = "Chế độ dinh dưỡng cho mèo trưởng thành",
                NoiDung = "Mèo trưởng thành cần khẩu phần giàu đạm động vật, hạn chế tinh bột. Nên chia 2-3 bữa/ngày, " +
                          "luôn có nước sạch, và tham khảo ý kiến bác sĩ thú y trước khi đổi loại thức ăn đột ngột để tránh " +
                          "rối loạn tiêu hóa.",
                ChuDe = "Mèo",
                TacGiaId = 1,
                AnhDaiDien = null,
                NgayDang = new DateTime(2026, 1, 3)
            },
            new BlogPost
            {
                PostId = 3,
                TieuDe = "Cách phòng bệnh thường gặp ở thú cưng theo mùa",
                NoiDung = "Thời tiết giao mùa là lúc thú cưng dễ mắc các bệnh về đường hô hấp và tiêu hóa. Chủ nuôi nên " +
                          "giữ ấm, vệ sinh nơi ở sạch sẽ, tiêm phòng định kỳ và quan sát các dấu hiệu bất thường như bỏ ăn, " +
                          "nôn mửa để đưa đi khám kịp thời.",
                ChuDe = "Sức khỏe",
                TacGiaId = 1,
                AnhDaiDien = null,
                NgayDang = new DateTime(2026, 1, 4)
            }
        );
    }
}
