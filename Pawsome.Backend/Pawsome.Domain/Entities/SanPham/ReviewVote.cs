using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.SanPham;

// 1 khách chỉ được vote hữu ích/không hữu ích đúng 1 lần cho 1 đánh giá (UNIQUE review_id+user_id
// ở ReviewVoteConfiguration) - bấm lại cùng lựa chọn thì gỡ vote, bấm lựa chọn khác thì đổi vote.
public class ReviewVote
{
    public int ReviewVoteId { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public bool HuuIch { get; set; }
    public DateTime NgayTao { get; set; }

    public Review Review { get; set; } = null!;
    public User User { get; set; } = null!;
}
