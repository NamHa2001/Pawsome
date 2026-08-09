using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Chỉ còn action đọc (khách duyệt danh mục công khai) - phần thêm/sửa/xóa (quản trị) đã dời
// sang AdminProductController (Phần 5), đúng ranh giới ở Pawsome_KhungDuAn.md mục 2, ghi chú (*).
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<CategoryDto>.Fail("Không tìm thấy danh mục."));

        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }
}