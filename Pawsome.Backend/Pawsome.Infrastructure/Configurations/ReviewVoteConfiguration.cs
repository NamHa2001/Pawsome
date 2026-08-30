using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Infrastructure.Configurations;

public class ReviewVoteConfiguration : IEntityTypeConfiguration<ReviewVote>
{
    public void Configure(EntityTypeBuilder<ReviewVote> builder)
    {
        builder.ToTable("review_votes");
        builder.HasKey(v => v.ReviewVoteId);

        builder.Property(v => v.ReviewVoteId).HasColumnName("review_vote_id");
        builder.Property(v => v.ReviewId).HasColumnName("review_id").IsRequired();
        builder.Property(v => v.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(v => v.HuuIch).HasColumnName("huu_ich").IsRequired();
        builder.Property(v => v.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");

        // 1 khách chỉ 1 vote/đánh giá - đổi ý thì UPDATE dòng này, không cho tạo thêm dòng mới.
        builder.HasIndex(v => new { v.ReviewId, v.UserId }).IsUnique().HasDatabaseName("UQ_review_votes_review_user");

        builder.HasOne(v => v.Review)
               .WithMany(r => r.Votes)
               .HasForeignKey(v => v.ReviewId)
               .HasConstraintName("FK_review_votes_reviews")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId)
               .HasConstraintName("FK_review_votes_users")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
