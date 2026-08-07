using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.SanPham;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class CategoryService : ICategoryService
{
    private readonly PawsomeDbContext _dbContext;

    public CategoryService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _dbContext.Categories
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                TenDanhMuc = c.TenDanhMuc,
                DanhMucChaId = c.DanhMucChaId,
                MoTa = c.MoTa
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            TenDanhMuc = category.TenDanhMuc,
            DanhMucChaId = category.DanhMucChaId,
            MoTa = category.MoTa
        };
    }

    public async Task<CategoryDto> CreateAsync(CategoryRequestDto dto)
    {
        var category = new Category
        {
            TenDanhMuc = dto.TenDanhMuc,
            DanhMucChaId = dto.DanhMucChaId,
            MoTa = dto.MoTa
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            TenDanhMuc = category.TenDanhMuc,
            DanhMucChaId = category.DanhMucChaId,
            MoTa = category.MoTa
        };
    }

    public async Task<CategoryDto> UpdateAsync(int id, CategoryRequestDto dto)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
            throw new InvalidOperationException("Không tìm thấy danh mục.");

        category.TenDanhMuc = dto.TenDanhMuc;
        category.DanhMucChaId = dto.DanhMucChaId;
        category.MoTa = dto.MoTa;

        await _dbContext.SaveChangesAsync();

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            TenDanhMuc = category.TenDanhMuc,
            DanhMucChaId = category.DanhMucChaId,
            MoTa = category.MoTa
        };
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
            throw new InvalidOperationException("Không tìm thấy danh mục.");

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }
}