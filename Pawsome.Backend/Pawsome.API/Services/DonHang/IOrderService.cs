using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;

namespace Pawsome.API.Services.DonHang;

public interface IOrderService
{
    Task<OrderDto> CreateFromCartAsync(int userId, CreateOrderRequestDto dto);
    Task<PagedResult<OrderDto>> GetByUserAsync(int userId, OrderFilterRequestDto filter);
    Task<OrderDto?> GetByIdAsync(int userId, int orderId);

    // Dành cho trang quản trị (Phần 5) - không giới hạn theo userId, vì admin cần xem/sửa
    // đơn của bất kỳ khách hàng nào, không riêng đơn của chính mình.
    Task<PagedResult<OrderDto>> GetAllAsync(OrderFilterRequestDto filter);
    Task<OrderDto?> GetByIdAdminAsync(int orderId);
    Task<OrderDto> CancelAsync(int userId, int orderId, CancelOrderRequestDto dto);
    Task<OrderDto> UpdateTrangThaiAsync(int orderId, string trangThaiMoi);
    Task<OrderDto> CapNhatVanDonAsync(int orderId, UpdateShippingRequestDto dto);
    Task<OrderDto> YeuCauTraHangAsync(int userId, int orderId, ReturnRequestDto dto);
    Task<OrderDto> DuyetTraHangAsync(int orderId, bool dongY);

    // Dùng bởi ReviewService (Phần 2) để chặn "verified purchase" - chỉ khách đã mua VÀ đã nhận
    // hàng (trạng thái da_giao) mới được đánh giá sản phẩm đó, xem Pawsome_KhungDuAn.md mục 2:
    // Phần 2 không tự query bảng orders, phải gọi qua IOrderService của Phần 4.
    Task<bool> DaMuaVaNhanHangAsync(int userId, int productId);

    // Tổng tiền đã tiết kiệm được (coupon + PawPoints + PawVip cộng gộp - Order.GiamGia đã là
    // tổng của cả 3 nguồn giảm giá đó tính lúc tạo đơn) - dùng cho dashboard khách hàng.
    Task<decimal> TongTietKiemAsync(int userId);
}