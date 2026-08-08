using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

// Gửi/duyệt đánh giá. DuyetAsync/TuChoiAsync dùng chung cho Phần 5 gọi khi kiểm duyệt
// (xem Pawsome_KhungDuAn.md mục 5.1 - Phần 5 không tự ghi thẳng vào bảng reviews).
public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetByProductAsync(int productId, int page, int pageSize);
    Task<List<ReviewDto>> GetChoDuyetAsync();
    Task<ReviewDto> CreateAsync(int userId, ReviewRequestDto dto);
    Task<ReviewDto> DuyetAsync(int reviewId);
    Task<ReviewDto> TuChoiAsync(int reviewId);
}
