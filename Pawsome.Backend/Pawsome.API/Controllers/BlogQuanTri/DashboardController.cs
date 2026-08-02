using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Thống kê doanh thu, đơn hàng, sản phẩm bán chạy. Chỉ đọc, không ghi
// (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
public class DashboardController : ControllerBase
{
}
