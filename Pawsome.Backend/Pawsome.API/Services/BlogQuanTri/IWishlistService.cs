using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;

namespace Pawsome.API.Services.BlogQuanTri;

public interface IWishlistService
{
    Task<PagedResult<WishlistItemDto>> GetByUserAsync(int userId, int page, int pageSize);
    Task<WishlistItemDto> AddAsync(int userId, int productId);
    Task RemoveAsync(int userId, int productId);
}
