using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.GioHang;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    // TODO (Phần 3): cần thêm endpoint GET công khai "coupon đang hiệu lực" để
    // trang chủ của Phần 2 gọi vào - xem Pawsome_KhungDuAn.md mục 5.9
}
