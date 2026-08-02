using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Kiểm duyệt đánh giá (duyệt/từ chối). Phải gọi IReviewService của Phần 2 - không
// tự viết data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
public class AdminReviewController : ControllerBase
{
}
