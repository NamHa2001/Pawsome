using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.Services.DonHang;

namespace Pawsome.API.Services.BlogQuanTri;

// Xem/cập nhật trạng thái đơn hàng cho trang quản trị. Chỉ gọi IOrderService của Phần 4,
// không tự query bảng orders (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
public class AdminOrderService : IAdminOrderService
{
    private readonly IOrderService _orderService;

    public AdminOrderService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public Task<PagedResult<OrderDto>> GetAllAsync(OrderFilterRequestDto filter) =>
        _orderService.GetAllAsync(filter);

    public Task<OrderDto?> GetByIdAsync(int orderId) =>
        _orderService.GetByIdAdminAsync(orderId);

    public Task<OrderDto> UpdateStatusAsync(int orderId, string trangThaiMoi) =>
        _orderService.UpdateTrangThaiAsync(orderId, trangThaiMoi);

    public Task<OrderDto> DuyetTraHangAsync(int orderId, bool dongY) =>
        _orderService.DuyetTraHangAsync(orderId, dongY);

    public Task<OrderDto> CapNhatVanDonAsync(int orderId, UpdateShippingRequestDto dto) =>
        _orderService.CapNhatVanDonAsync(orderId, dto);
}
