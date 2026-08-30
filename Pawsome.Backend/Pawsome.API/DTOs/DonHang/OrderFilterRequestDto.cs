namespace Pawsome.API.DTOs.DonHang;

public class OrderFilterRequestDto
{
    public string? TrangThai { get; set; }
    // Chỉ dùng ở AdminOrderController.GetAll (Authorize Admin) để lọc đơn theo 1 khách cụ thể -
    // luồng khách tự xem đơn của mình (OrdersController.GetMyOrders) không dùng field này, luôn
    // gọi GetByUserAsync với CurrentUserId riêng nên không có rủi ro xem chéo đơn người khác.
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}