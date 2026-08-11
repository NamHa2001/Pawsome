using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;

namespace Pawsome.API.Services.DonHang;

public interface IOrderService
{
    Task<OrderDto> CreateFromCartAsync(int userId, CreateOrderRequestDto dto);
    Task<PagedResult<OrderDto>> GetByUserAsync(int userId, OrderFilterRequestDto filter);
    Task<OrderDto?> GetByIdAsync(int userId, int orderId);
    Task<OrderDto> CancelAsync(int userId, int orderId, CancelOrderRequestDto dto);
    Task<OrderDto> UpdateTrangThaiAsync(int orderId, string trangThaiMoi);
}