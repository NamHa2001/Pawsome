using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryRequestDto dto);
    Task<CategoryDto> UpdateAsync(int id, CategoryRequestDto dto);
    Task DeleteAsync(int id);
}
