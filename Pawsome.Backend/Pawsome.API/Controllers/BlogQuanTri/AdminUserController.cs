using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Quản lý tài khoản, phân quyền. Phải gọi IUserService của Phần 1 - không tự viết
// data access riêng (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
public class AdminUserController : ControllerBase
{
}
