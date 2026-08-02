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
    }
}
