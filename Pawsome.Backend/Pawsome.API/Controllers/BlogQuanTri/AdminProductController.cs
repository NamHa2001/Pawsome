using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Thêm/sửa/xóa sản phẩm, tồn kho, danh mục, thương hiệu. Phải gọi IProductService/
// ICategoryService/IBrandService của Phần 2 - không tự viết data access riêng
// (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
// Các action ghi (POST/PUT/DELETE) trước đây nằm ở ProductsController/CategoriesController/
// BrandsController (Phần 2) đã được dời về đây cho đúng ranh giới tài liệu - 3 controller đó
// giờ chỉ còn action đọc (GET) phục vụ khách duyệt hàng công khai.
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;

    public AdminProductController(
        IProductService productService,
        ICategoryService categoryService,
        IBrandService brandService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
    }

    // ── Sản phẩm ───────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto dto)
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
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateRequestDto dto)
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
    public async Task<IActionResult> DeleteProduct(int id)
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

    // ── Biến thể / tồn kho ─────────────────────────────────────────────────

    [HttpPost("{id}/Variants")]
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

    // ── Hình ảnh ───────────────────────────────────────────────────────────

    [HttpPost("{id}/Images")]
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

    // ── Danh mục ───────────────────────────────────────────────────────────

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDto dto)
    {
        try
        {
            var result = await _categoryService.CreateAsync(dto);
            return Ok(ApiResponse<CategoryDto>.Ok(result, "Tạo danh mục thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequestDto dto)
    {
        try
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return Ok(ApiResponse<CategoryDto>.Ok(result, "Cập nhật thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Xóa thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ── Thương hiệu ────────────────────────────────────────────────────────

    [HttpPost("brands")]
    public async Task<IActionResult> CreateBrand([FromBody] BrandRequestDto dto)
    {
        try
        {
            var result = await _brandService.CreateAsync(dto);
            return Ok(ApiResponse<BrandDto>.Ok(result, "Tạo thương hiệu thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BrandDto>.Fail(ex.Message));
        }
    }

    [HttpPut("brands/{id}")]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandRequestDto dto)
    {
        try
        {
            var result = await _brandService.UpdateAsync(id, dto);
            return Ok(ApiResponse<BrandDto>.Ok(result, "Cập nhật thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BrandDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BrandDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("brands/{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        try
        {
            await _brandService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Xóa thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
