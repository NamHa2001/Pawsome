using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Chỉ còn action đọc (khách tìm kiếm/duyệt sản phẩm công khai) - phần thêm/sửa/xóa sản phẩm,
// biến thể, hình ảnh (quản trị) đã dời sang AdminProductController (Phần 5), đúng ranh giới ở
// Pawsome_KhungDuAn.md mục 2, ghi chú (*).
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

    [HttpGet("{id}/ban-chay")]
    public async Task<IActionResult> GetBanChay(int id, [FromQuery] int soLuong = 4)
    {
        var result = await _productService.GetBanChayAsync(id, soLuong);
        return Ok(ApiResponse<List<ProductDto>>.Ok(result));
    }
}
