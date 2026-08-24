using Microsoft.Data.SqlClient;
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
        if (dto.DanhMucChaId.HasValue)
        {
            var chaTonTai = await _dbContext.Categories.AnyAsync(c => c.CategoryId == dto.DanhMucChaId.Value);
            if (!chaTonTai)
                throw new InvalidOperationException("Danh mục cha không tồn tại.");
        }

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
        // KiemTraVongLapAsync là kiểu "đọc rồi mới ghi" (check-then-write): nếu 2 admin cùng sửa cha
        // của 2 danh mục khác nhau gần như đồng thời (VD: sửa A thành con của B, đúng lúc có request
        // khác sửa B thành con của A), ở mức cô lập Read Committed mặc định cả 2 request đều đọc dữ
        // liệu CŨ (chưa thấy thay đổi của nhau) nên đều qua được bước kiểm tra, rồi cả 2 đều lưu thành
        // công -> tạo ra vòng lặp thật A->B->A. Bọc trong transaction Serializable để SQL Server khóa
        // đúng các dòng đã đọc, buộc request đến sau phải chờ (hoặc bị hủy nếu có deadlock) thay vì
        // cùng lúc "mù" về nhau.
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Không tìm thấy danh mục.");

            if (dto.DanhMucChaId.HasValue)
            {
                var chaTonTai = await _dbContext.Categories.AnyAsync(c => c.CategoryId == dto.DanhMucChaId.Value);
                if (!chaTonTai)
                    throw new InvalidOperationException("Danh mục cha không tồn tại.");

                await KiemTraVongLapAsync(id, dto.DanhMucChaId.Value);
            }

            category.TenDanhMuc = dto.TenDanhMuc;
            category.DanhMucChaId = dto.DanhMucChaId;
            category.MoTa = dto.MoTa;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                TenDanhMuc = category.TenDanhMuc,
                DanhMucChaId = category.DanhMucChaId,
                MoTa = category.MoTa
            };
        }
        catch (SqlException ex) when (ex.Number == 1205)
        {
            // 1205 = deadlock victim: đúng lúc 2 request cùng đụng transaction Serializable ở trên,
            // SQL Server tự hủy 1 trong 2 để tránh treo nhau mãi. Báo lỗi nghiệp vụ để controller trả
            // 400 gọn thay vì để lộ ra thành lỗi 500. Deadlock có thể lộ ra ở 2 dạng: SqlException thô
            // (nếu xảy ra lúc đọc trong KiemTraVongLapAsync hoặc lúc CommitAsync) hoặc bọc trong
            // DbUpdateException (nếu xảy ra ngay trong SaveChangesAsync - EF Core luôn bọc exception
            // của provider trong DbUpdateException ở bước này, giống hệt trường hợp SKU trùng ở
            // ProductService.LaLoiViPhamUniqueSku) - nên phải bắt cả 2 dạng, bắt mỗi SqlException là
            // sót đúng trường hợp thực tế hay xảy ra nhất.
            throw new InvalidOperationException("Có request khác đang sửa cùng danh mục này, vui lòng thử lại.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 1205)
        {
            throw new InvalidOperationException("Có request khác đang sửa cùng danh mục này, vui lòng thử lại.");
        }
    }

    // Duyệt ngược từ danhMucChaMoiId lên tới gốc; nếu gặp lại chinhId nghĩa là chinhId đang là tổ
    // tiên của danh mục cha mới -> gán sẽ tạo vòng lặp (không chỉ chặn trường hợp cha trực tiếp
    // là chính nó, mà cả các vòng nhiều cấp như A->B rồi sau đó sửa A thành con của B).
    private async Task KiemTraVongLapAsync(int chinhId, int danhMucChaMoiId)
    {
        int? currentId = danhMucChaMoiId;
        var daDuyet = new HashSet<int>();

        while (currentId.HasValue)
        {
            if (currentId.Value == chinhId)
                throw new InvalidOperationException("Không thể chọn danh mục con (hoặc cháu) của chính nó làm danh mục cha - sẽ tạo vòng lặp.");

            if (!daDuyet.Add(currentId.Value))
                break; // Vòng lặp có sẵn từ trước nằm phía trên, không liên quan lần sửa này - tránh lặp vô hạn.

            currentId = await _dbContext.Categories
                .Where(c => c.CategoryId == currentId.Value)
                .Select(c => c.DanhMucChaId)
                .FirstOrDefaultAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
            throw new KeyNotFoundException("Không tìm thấy danh mục.");

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }
}