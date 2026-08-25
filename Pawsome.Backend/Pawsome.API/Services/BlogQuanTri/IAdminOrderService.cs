using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;

namespace Pawsome.API.Services.BlogQuanTri;

public interface IAdminOrderService
{
    Task<PagedResult<OrderDto>> GetAllAsync(OrderFilterRequestDto filter);
    Task<OrderDto?> GetByIdAsync(int orderId);
    Task<OrderDto> UpdateStatusAsync(int orderId, string trangThaiMoi);
}
