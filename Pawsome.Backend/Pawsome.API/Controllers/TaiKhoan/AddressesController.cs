using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.API.Services.TaiKhoan;

namespace Pawsome.API.Controllers.TaiKhoan;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _addressService.GetAllAsync(CurrentUserId);
        return Ok(ApiResponse<List<AddressDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequestDto dto)
    {
        var result = await _addressService.CreateAsync(CurrentUserId, dto);
        return Ok(ApiResponse<AddressDto>.Ok(result, "Thêm địa chỉ thành công."));
    }

    [HttpPut("{addressId}")]
    public async Task<IActionResult> Update(int addressId, [FromBody] UpdateAddressRequestDto dto)
    {
        try
        {
            var result = await _addressService.UpdateAsync(CurrentUserId, addressId, dto);
            return Ok(ApiResponse<AddressDto>.Ok(result, "Cập nhật địa chỉ thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<AddressDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> Delete(int addressId)
    {
        try
        {
            await _addressService.DeleteAsync(CurrentUserId, addressId);
            return Ok(ApiResponse<object>.Ok(null!, "Xóa địa chỉ thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}