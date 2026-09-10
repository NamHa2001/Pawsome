namespace Pawsome.API.DTOs.AI;

public class ChatResponseDto
{
    public string TraLoi { get; set; } = null!;

    // Có thể vẫn có giá trị dù Loi=true (VD: gọi hàm tra cứu thành công 1-2 lượt rồi mới gặp lỗi ở
    // lượt kế tiếp) - không suy luận "SanPham khác rỗng" nghĩa là TraLoi chắc chắn là câu trả lời
    // thành công, phải luôn kiểm tra Loi riêng.
    public List<SanPhamGoiYDto> SanPham { get; set; } = new();

    // true nếu TraLoi là thông báo lỗi/từ chối (TraLoiKhiLoi) thay vì câu trả lời AI tổng hợp thật -
    // Frontend dùng để hiển thị khác đi (không lẫn giữa bong bóng lỗi và bong bóng trả lời thành công).
    public bool Loi { get; set; }
}
