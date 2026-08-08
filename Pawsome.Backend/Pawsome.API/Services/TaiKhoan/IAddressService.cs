using Pawsome.API.DTOs.TaiKhoan;

namespace Pawsome.API.Services.TaiKhoan;

// Địa chỉ giao hàng
public interface IAddressService
{
    Task<List<AddressDto>> GetAllAsync(int userId);
    Task<AddressDto> CreateAsync(int userId, CreateAddressRequestDto dto);
    Task<AddressDto> UpdateAsync(int userId, int addressId, UpdateAddressRequestDto dto);
    Task DeleteAsync(int userId, int addressId);
}