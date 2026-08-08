using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ProductFilterRequestDto filter)
    {
        var result = await _productService.SearchAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string? tuKhoa, [FromQuery] int soLuong = 8)
    {
        var result = await _productService.GetSuggestionsAsync(tuKhoa ?? "", soLuong);
        return Ok(ApiResponse<List<ProductSuggestionDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<ProductDto>.Fail("Không tìm thấy sản phẩm."));

        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ProductRequestDto dto)
    {
        try
        {
            var result = await _productService.CreateAsync(dto);
            return Ok(ApiResponse<ProductDto>.Ok(result, "Tạo sản phẩm thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateRequestDto dto)
    {
        try
        {
            var result = await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ProductDto>.Ok(result, "Cập nhật thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Đã ẩn sản phẩm."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/Variants")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddVariant(int id, [FromBody] ProductVariantRequestDto dto)
    {
        try
        {
            var result = await _productService.AddVariantAsync(id, dto);
            return Ok(ApiResponse<ProductVariantDto>.Ok(result, "Thêm biến thể thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductVariantDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductVariantDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/Variants/{variantId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVariant(int id, int variantId, [FromBody] ProductVariantRequestDto dto)
    {
        try
        {
            var result = await _productService.UpdateVariantAsync(id, variantId, dto);
            return Ok(ApiResponse<ProductVariantDto>.Ok(result, "Cập nhật biến thể thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductVariantDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductVariantDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}/Variants/{variantId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVariant(int id, int variantId)
    {
        try
        {
            await _productService.DeleteVariantAsync(id, variantId);
            return Ok(ApiResponse<object>.Ok(new { }, "Đã ẩn biến thể."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/Images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int id, [FromBody] ProductImageRequestDto dto)
    {
        try
        {
            var result = await _productService.AddImageAsync(id, dto);
            return Ok(ApiResponse<ProductImageDto>.Ok(result, "Thêm ảnh thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductImageDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}/Images/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        try
        {
            await _productService.DeleteImageAsync(id, imageId);
            return Ok(ApiResponse<object>.Ok(new { }, "Xóa ảnh thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
