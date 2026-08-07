using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

public interface IBrandService
{
    Task<List<BrandDto>> GetAllAsync();
    Task<BrandDto?> GetByIdAsync(int id);
    Task<BrandDto> CreateAsync(BrandRequestDto dto);
    Task<BrandDto> UpdateAsync(int id, BrandRequestDto dto);
    Task DeleteAsync(int id);
}
