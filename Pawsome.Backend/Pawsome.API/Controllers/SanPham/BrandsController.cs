using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BrandRequestDto dto)
    {
        var result = await _brandService.CreateAsync(dto);
        return Ok(ApiResponse<BrandDto>.Ok(result, "Tạo thương hiệu thành công."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BrandRequestDto dto)
    {
        try
        {
            var result = await _brandService.UpdateAsync(id, dto);
            return Ok(ApiResponse<BrandDto>.Ok(result, "Cập nhật thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<BrandDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _brandService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Xóa thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
