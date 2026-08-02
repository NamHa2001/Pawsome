using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Xem/cập nhật trạng thái đơn hàng. Phải gọi IOrderService của Phần 4 - không tự
// viết data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
public class AdminOrderController : ControllerBase
{
}
