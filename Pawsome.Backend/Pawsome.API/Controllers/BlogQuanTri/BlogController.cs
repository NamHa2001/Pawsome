using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.Services.BlogQuanTri;

namespace Pawsome.API.Controllers.BlogQuanTri;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] BlogFilterRequestDto filter)
    {
        var result = await _blogService.SearchAsync(filter);
        return Ok(ApiResponse<PagedResult<BlogPostDto>>.Ok(result));
    }

    [HttpGet("chu-de")]
    public async Task<IActionResult> GetChuDeList()
    {
        var result = await _blogService.GetChuDeListAsync();
        return Ok(ApiResponse<List<string>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _blogService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<BlogPostDto>.Fail("Không tìm thấy bài viết."));

        return Ok(ApiResponse<BlogPostDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] BlogPostRequestDto dto)
    {
        try
        {
            var result = await _blogService.CreateAsync(CurrentUserId, dto);
            return Ok(ApiResponse<BlogPostDto>.Ok(result, "Đăng bài thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BlogPostDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] BlogPostRequestDto dto)
    {
        try
        {
            var result = await _blogService.UpdateAsync(id, dto);
            return Ok(ApiResponse<BlogPostDto>.Ok(result, "Cập nhật bài viết thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BlogPostDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _blogService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Đã xóa bài viết."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
