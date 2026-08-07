using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryRequestDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "Tạo danh mục thành công."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryRequestDto dto)
    {
        try
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return Ok(ApiResponse<CategoryDto>.Ok(result, "Cập nhật thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Xóa thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}