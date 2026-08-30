using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

// Gửi/duyệt đánh giá. DuyetAsync/TuChoiAsync dùng chung cho Phần 5 gọi khi kiểm duyệt
// (xem Pawsome_KhungDuAn.md mục 5.1 - Phần 5 không tự ghi thẳng vào bảng reviews).
public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetByProductAsync(int productId, int page, int pageSize, int? currentUserId = null);

    // Đánh giá tiêu biểu cho trang chủ (YCGD-5.1, mục "Đánh giá tiêu biểu của khách hàng") - lấy
    // các đánh giá đã duyệt, có bình luận, điểm cao nhất trước, không phân biệt sản phẩm.
    Task<List<ReviewDto>> GetNoiBatAsync(int soLuong);
    Task<List<ReviewDto>> GetChoDuyetAsync();
    Task<ReviewDto> CreateAsync(int userId, ReviewRequestDto dto);

    // "Verified purchase" - chỉ khách đã mua VÀ đã nhận hàng (đơn ở trạng thái da_giao) mới được
    // đánh giá sản phẩm đó. Frontend gọi để quyết định hiện form gửi đánh giá hay hiện thông báo
    // "cần mua trước", CreateAsync cũng tự kiểm tra lại (không chỉ tin tưởng phía client).
    Task<bool> CoTheDanhGiaAsync(int userId, int productId);
    Task<ReviewDto> DuyetAsync(int reviewId);
    Task<ReviewDto> TuChoiAsync(int reviewId);

    // Vote hữu ích/không hữu ích cho 1 đánh giá đã duyệt - bấm lại đúng lựa chọn cũ thì gỡ vote,
    // bấm lựa chọn khác thì đổi vote (xem ReviewVote.cs).
    Task<ReviewDto> VoteAsync(int userId, int reviewId, bool huuIch);
}
