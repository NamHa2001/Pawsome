using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Chỉ còn action đọc (khách duyệt thương hiệu công khai) - phần thêm/sửa/xóa (quản trị) đã dời
// sang AdminProductController (Phần 5), đúng ranh giới ở Pawsome_KhungDuAn.md mục 2, ghi chú (*).
[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandService.GetAllAsync();
        return Ok(ApiResponse<List<BrandDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _brandService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<BrandDto>.Fail("Không tìm thấy thương hiệu."));

        return Ok(ApiResponse<BrandDto>.Ok(result));
    }
}
