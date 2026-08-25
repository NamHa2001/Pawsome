using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

// Gửi/duyệt đánh giá. DuyetAsync/TuChoiAsync dùng chung cho Phần 5 gọi khi kiểm duyệt
// (xem Pawsome_KhungDuAn.md mục 5.1 - Phần 5 không tự ghi thẳng vào bảng reviews).
public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetByProductAsync(int productId, int page, int pageSize);

    // Đánh giá tiêu biểu cho trang chủ (YCGD-5.1, mục "Đánh giá tiêu biểu của khách hàng") - lấy
    // các đánh giá đã duyệt, có bình luận, điểm cao nhất trước, không phân biệt sản phẩm.
    Task<List<ReviewDto>> GetNoiBatAsync(int soLuong);
    Task<List<ReviewDto>> GetChoDuyetAsync();
    Task<ReviewDto> CreateAsync(int userId, ReviewRequestDto dto);
    Task<ReviewDto> DuyetAsync(int reviewId);
    Task<ReviewDto> TuChoiAsync(int reviewId);
}
