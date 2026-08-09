using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;

namespace Pawsome.API.Services.BlogQuanTri;

// Đăng bài, tìm kiếm/phân loại theo chủ đề (YC-8.1, YC-8.2). Chia sẻ mạng xã hội (YC-8.3)
// là hành vi thuần Frontend (nút chia sẻ link), không cần API riêng.
public interface IBlogService
{
    Task<PagedResult<BlogPostDto>> SearchAsync(BlogFilterRequestDto filter);
    Task<List<string>> GetChuDeListAsync();
    Task<BlogPostDto?> GetByIdAsync(int id);
    Task<BlogPostDto> CreateAsync(int tacGiaId, BlogPostRequestDto dto);
    Task<BlogPostDto> UpdateAsync(int id, BlogPostRequestDto dto);
    Task DeleteAsync(int id);
}
