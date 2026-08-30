using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.SanPham;

public class Review
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public byte SoSao { get; set; }
    public string? BinhLuan { get; set; }
    public string TrangThai { get; set; } = "cho_duyet";
    public DateTime NgayTao { get; set; }

    // 3 chỉ số phụ - chỉ có ở đánh giá gửi từ sau khi tính năng này được thêm vào (form bắt
    // buộc chấm cả 3 mục). Đánh giá cũ hơn không có dữ liệu này nên để NULL, không bịa số.
    public byte? DiemChatLuong { get; set; }
    public byte? DiemGiaTri { get; set; }
    public byte? DiemHaiLongThuCung { get; set; }

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
}
