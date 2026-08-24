using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Chỉ có action đọc - danh sách tình trạng sức khỏe cố định (menu "Shop by Condition"),
// dùng cho bộ lọc sản phẩm (YC-2.3). Không có thêm/sửa/xóa vì danh sách 6 mục đã cố định
// theo giao diện, không phải dữ liệu quản trị viên tự thêm.
[ApiController]
[Route("api/[controller]")]
public class ConditionsController : ControllerBase
{
    private readonly IConditionService _conditionService;

    public ConditionsController(IConditionService conditionService)
    {
        _conditionService = conditionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _conditionService.GetAllAsync();
        return Ok(ApiResponse<List<ConditionDto>>.Ok(result));
    }
}
