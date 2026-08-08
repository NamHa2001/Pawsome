using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.SanPham;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class BrandService : IBrandService
{
    private readonly PawsomeDbContext _dbContext;

    public BrandService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BrandDto>> GetAllAsync()
    {
        return await _dbContext.Brands
            .Select(b => new BrandDto
            {
                BrandId = b.BrandId,
                TenThuongHieu = b.TenThuongHieu,
                LogoUrl = b.LogoUrl
            })
            .ToListAsync();
    }

    public async Task<BrandDto?> GetByIdAsync(int id)
    {
        var brand = await _dbContext.Brands.FindAsync(id);
        if (brand == null) return null;

        return new BrandDto
        {
            BrandId = brand.BrandId,
            TenThuongHieu = brand.TenThuongHieu,
            LogoUrl = brand.LogoUrl
        };
    }

    public async Task<BrandDto> CreateAsync(BrandRequestDto dto)
    {
        var trungTen = await _dbContext.Brands.AnyAsync(b => b.TenThuongHieu == dto.TenThuongHieu);
        if (trungTen)
            throw new InvalidOperationException("Tên thương hiệu này đã tồn tại.");

        var brand = new Brand
        {
            TenThuongHieu = dto.TenThuongHieu,
            LogoUrl = dto.LogoUrl
        };

        _dbContext.Brands.Add(brand);
        await _dbContext.SaveChangesAsync();

        return new BrandDto
        {
            BrandId = brand.BrandId,
            TenThuongHieu = brand.TenThuongHieu,
            LogoUrl = brand.LogoUrl
        };
    }

    public async Task<BrandDto> UpdateAsync(int id, BrandRequestDto dto)
    {
        var brand = await _dbContext.Brands.FindAsync(id);
        if (brand == null)
            throw new KeyNotFoundException("Không tìm thấy thương hiệu.");

        var trungTen = await _dbContext.Brands.AnyAsync(b => b.TenThuongHieu == dto.TenThuongHieu && b.BrandId != id);
        if (trungTen)
            throw new InvalidOperationException("Tên thương hiệu này đã tồn tại.");

        brand.TenThuongHieu = dto.TenThuongHieu;
        brand.LogoUrl = dto.LogoUrl;

        await _dbContext.SaveChangesAsync();

        return new BrandDto
        {
            BrandId = brand.BrandId,
            TenThuongHieu = brand.TenThuongHieu,
            LogoUrl = brand.LogoUrl
        };
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _dbContext.Brands.FindAsync(id);
        if (brand == null)
            throw new KeyNotFoundException("Không tìm thấy thương hiệu.");

        _dbContext.Brands.Remove(brand);
        await _dbContext.SaveChangesAsync();
    }
}
