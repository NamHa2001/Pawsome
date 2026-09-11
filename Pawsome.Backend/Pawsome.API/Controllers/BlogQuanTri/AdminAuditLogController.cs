using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.Common.AuditLog;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Trang "Nhật ký hệ thống" cho quản trị viên (SRS mục 6.4 "Ghi nhật ký") - chỉ đọc, không sửa
// bảng audit_logs. Gọi thẳng IAuditLogService (Common/) vì đây là dữ liệu dùng chung toàn hệ
// thống, không thuộc riêng Phần nào, giống cách trang quản trị dùng chung Common/AI.
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminAuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AdminAuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AuditLogFilterDto filter)
    {
        var result = await _auditLogService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<AuditLogDto>>.Ok(result));
    }
}
