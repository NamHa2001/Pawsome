using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;
using Pawsome.Domain.Entities.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class ProductService : IProductService
{
    private readonly PawsomeDbContext _dbContext;

    public ProductService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(ProductFilterRequestDto filter)
    {
        var query = _dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.TrangThai == "da_duyet"))
            .Include(p => p.ProductConditions).ThenInclude(pc => pc.Condition)
            .Where(p => p.DangKinhDoanh)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.ConditionId.HasValue)
            query = query.Where(p => p.ProductConditions.Any(pc => pc.ConditionId == filter.ConditionId.Value));

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            query = query.Where(p => p.Ten.Contains(filter.TuKhoa.Trim()));

        if (filter.GiaMin.HasValue)
            query = query.Where(p => p.GiaTu != null && p.GiaTu >= filter.GiaMin.Value);

        if (filter.GiaMax.HasValue)
            query = query.Where(p => p.GiaTu != null && p.GiaTu <= filter.GiaMax.Value);

        if (filter.DanhGiaMin.HasValue)
            query = query.Where(p => p.DiemDanhGiaTb >= filter.DanhGiaMin.Value);

        var tongSo = await query.CountAsync();

        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 20 : Math.Min(filter.PageSize, 100);

        var items = await query
            .OrderByDescending(p => p.NgayTao)
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    // Gợi ý tự động khi gõ tìm kiếm (YC-2.4) - trả về nhanh, danh sách rút gọn, ưu tiên
    // tên bắt đầu bằng từ khóa trước, chỉ lấy sản phẩm đang kinh doanh.
    public async Task<List<ProductSuggestionDto>> GetSuggestionsAsync(string tuKhoa, int soLuong)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa))
            return new List<ProductSuggestionDto>();

        var tuKhoaSach = tuKhoa.Trim();
        var gioiHan = soLuong < 1 ? 8 : Math.Min(soLuong, 20);

        var items = await _dbContext.Products
            .Include(p => p.Images)
            .Where(p => p.DangKinhDoanh && p.Ten.Contains(tuKhoaSach))
            .OrderBy(p => p.Ten.StartsWith(tuKhoaSach) ? 0 : 1)
            .ThenBy(p => p.Ten)
            .Take(gioiHan)
            .ToListAsync();

        return items.Select(p => new ProductSuggestionDto
        {
            ProductId = p.ProductId,
            Ten = p.Ten,
            AnhChinh = p.Images.FirstOrDefault(i => i.LaAnhChinh)?.Url ?? p.Images.FirstOrDefault()?.Url,
            GiaTu = p.GiaTu
        }).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        // Route công khai (ProductsController, không yêu cầu đăng nhập) - phải lọc dangKinhDoanh
        // giống hệt SearchAsync, không thì sản phẩm đã bị admin ẩn (soft-delete) vẫn xem/mua được
        // qua link cũ.
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.TrangThai == "da_duyet"))
            .Include(p => p.ProductConditions).ThenInclude(pc => pc.Condition)
            .FirstOrDefaultAsync(p => p.ProductId == id && p.DangKinhDoanh);

        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(ProductRequestDto dto)
    {
        var danhMucTonTai = await _dbContext.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!danhMucTonTai)
            throw new InvalidOperationException("Danh mục không tồn tại.");

        if (dto.BrandId.HasValue)
        {
            var thuongHieuTonTai = await _dbContext.Brands.AnyAsync(b => b.BrandId == dto.BrandId.Value);
            if (!thuongHieuTonTai)
                throw new InvalidOperationException("Thương hiệu không tồn tại.");
        }

        await KiemTraTrungSkuAsync(dto.Variants, null);
        var danhSachCondition = await LayDanhSachConditionHopLeAsync(dto.ConditionIds);

        var product = new Product
        {
            Ten = dto.Ten,
            MoTa = dto.MoTa,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            LieuLuong = dto.LieuLuong,
            NgayTao = DateTime.UtcNow,
            NgayCapNhat = DateTime.UtcNow
        };

        foreach (var condition in danhSachCondition)
        {
            product.ProductConditions.Add(new ProductCondition { Condition = condition });
        }

        foreach (var v in dto.Variants)
        {
            product.Variants.Add(new ProductVariant
            {
                TenBienThe = v.TenBienThe,
                Gia = v.Gia,
                SoLuongTon = v.SoLuongTon,
                Sku = v.Sku
            });
        }

        if (dto.Images != null)
        {
            foreach (var img in dto.Images)
            {
                product.Images.Add(new ProductImage
                {
                    Url = img.Url,
                    LaAnhChinh = img.LaAnhChinh
                });
            }
        }

        CapNhatGiaTu(product);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductUpdateRequestDto dto)
    {
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.TrangThai == "da_duyet"))
            .Include(p => p.ProductConditions).ThenInclude(pc => pc.Condition)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        var danhMucTonTai = await _dbContext.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!danhMucTonTai)
            throw new InvalidOperationException("Danh mục không tồn tại.");

        if (dto.BrandId.HasValue)
        {
            var thuongHieuTonTai = await _dbContext.Brands.AnyAsync(b => b.BrandId == dto.BrandId.Value);
            if (!thuongHieuTonTai)
                throw new InvalidOperationException("Thương hiệu không tồn tại.");
        }

        var danhSachCondition = await LayDanhSachConditionHopLeAsync(dto.ConditionIds);

        product.Ten = dto.Ten;
        product.MoTa = dto.MoTa;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.LieuLuong = dto.LieuLuong;

        // Thay toàn bộ danh sách tình trạng bằng danh sách mới gửi lên (dto.ConditionIds == null
        // nghĩa là không đổi field này trên client - giữ nguyên; gửi mảng rỗng mới là "gỡ hết").
        if (dto.ConditionIds != null)
        {
            product.ProductConditions.Clear();
            foreach (var condition in danhSachCondition)
            {
                product.ProductConditions.Add(new ProductCondition { Condition = condition });
            }
        }

        await _dbContext.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task DeleteAsync(int id)
    {
        // Soft-delete: xem ghi chú "GHI CHÚ PHIÊN BẢN" mục 2 trong Pawsome_Database.sql -
        // xóa cứng sẽ luôn thất bại một khi sản phẩm đã từng được đặt hàng.
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        product.DangKinhDoanh = false;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ProductVariantDto> AddVariantAsync(int productId, ProductVariantRequestDto dto)
    {
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        await KiemTraTrungSkuAsync(new List<ProductVariantRequestDto> { dto }, null);

        var variant = new ProductVariant
        {
            TenBienThe = dto.TenBienThe,
            Gia = dto.Gia,
            SoLuongTon = dto.SoLuongTon,
            Sku = dto.Sku
        };
        product.Variants.Add(variant);

        CapNhatGiaTu(product);
        await _dbContext.SaveChangesAsync();

        return MapVariantToDto(variant);
    }

    public async Task<ProductVariantDto> UpdateVariantAsync(int productId, int variantId, ProductVariantRequestDto dto)
    {
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        var variant = product.Variants.FirstOrDefault(v => v.VariantId == variantId);
        if (variant == null)
            throw new KeyNotFoundException("Không tìm thấy biến thể.");

        await KiemTraTrungSkuAsync(new List<ProductVariantRequestDto> { dto }, variantId);

        variant.TenBienThe = dto.TenBienThe;
        variant.Gia = dto.Gia;
        variant.SoLuongTon = dto.SoLuongTon;
        variant.Sku = dto.Sku;

        CapNhatGiaTu(product);
        await _dbContext.SaveChangesAsync();

        return MapVariantToDto(variant);
    }

    public async Task DeleteVariantAsync(int productId, int variantId)
    {
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        var variant = product.Variants.FirstOrDefault(v => v.VariantId == variantId);
        if (variant == null)
            throw new KeyNotFoundException("Không tìm thấy biến thể.");

        // Mỗi sản phẩm phải luôn còn tối thiểu 1 biến thể đang kinh doanh (xem Database.sql mục Quy ước)
        var soBienTheDangHoatDong = product.Variants.Count(v => v.DangKinhDoanh);
        if (variant.DangKinhDoanh && soBienTheDangHoatDong <= 1)
            throw new InvalidOperationException("Sản phẩm phải có ít nhất 1 biến thể đang kinh doanh.");

        variant.DangKinhDoanh = false;

        CapNhatGiaTu(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ProductImageDto> AddImageAsync(int productId, ProductImageRequestDto dto)
    {
        var product = await _dbContext.Products.FindAsync(productId);
        if (product == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

        var image = new ProductImage
        {
            ProductId = productId,
            Url = dto.Url,
            LaAnhChinh = dto.LaAnhChinh
        };

        _dbContext.ProductImages.Add(image);
        await _dbContext.SaveChangesAsync();

        return new ProductImageDto
        {
            ImageId = image.ImageId,
            Url = image.Url,
            LaAnhChinh = image.LaAnhChinh
        };
    }

    public async Task DeleteImageAsync(int productId, int imageId)
    {
        // Ảnh là dữ liệu con thuần túy (xem Database.sql) -> xóa cứng bình thường, không cần soft-delete
        var image = await _dbContext.ProductImages
            .FirstOrDefaultAsync(i => i.ImageId == imageId && i.ProductId == productId);

        if (image == null)
            throw new KeyNotFoundException("Không tìm thấy hình ảnh.");

        _dbContext.ProductImages.Remove(image);
        await _dbContext.SaveChangesAsync();
    }

    // Dùng ExecuteUpdateAsync (UPDATE ... WHERE so_luong_ton >= @soLuong trong đúng 1 câu SQL nguyên tử)
    // thay vì đọc-rồi-ghi qua change tracker: đọc-rồi-ghi bị "lost update" khi 2 đơn hàng trừ kho đồng
    // thời - cả hai đều đọc được tồn kho cũ, đều tính trừ thành công, ghi đè lên nhau, tồn kho cuối cùng
    // chỉ giảm 1 lần dù cả 2 đơn đều báo thành công (bán vượt tồn kho thật). ExecuteUpdateAsync để SQL
    // Server tự khóa dòng trong lúc UPDATE, request đến sau sẽ thấy tồn kho đã giảm và tự fail đúng.
    //
    // LƯU Ý cho nơi gọi (Phần 4 - xử lý đơn hàng): ExecuteUpdateAsync ghi thẳng xuống DB, KHÔNG cập
    // nhật entity ProductVariant nào đang được track sẵn trong cùng DbContext (nếu trước đó đã
    // FindAsync/Include variant này để đọc giá...). Nếu cần biết tồn kho MỚI sau khi gọi hàm này,
    // phải query lại (FindAsync lại hoặc AsNoTracking), không dùng property trên biến/entity đã đọc
    // trước đó - nó sẽ vẫn giữ giá trị cũ.
    public async Task TruTonKhoAsync(int variantId, int soLuong)
    {
        var soDongCapNhat = await _dbContext.ProductVariants
            .Where(v => v.VariantId == variantId && v.SoLuongTon >= soLuong)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(v => v.SoLuongTon, v => v.SoLuongTon - soLuong));

        if (soDongCapNhat > 0)
            return;

        var tonTai = await _dbContext.ProductVariants.AnyAsync(v => v.VariantId == variantId);
        if (!tonTai)
            throw new KeyNotFoundException("Không tìm thấy biến thể sản phẩm.");

        throw new InvalidOperationException("Số lượng tồn kho không đủ.");
    }

    public async Task HoanKhoAsync(int variantId, int soLuong)
    {
        var soDongCapNhat = await _dbContext.ProductVariants
            .Where(v => v.VariantId == variantId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(v => v.SoLuongTon, v => v.SoLuongTon + soLuong));

        if (soDongCapNhat == 0)
            throw new KeyNotFoundException("Không tìm thấy biến thể sản phẩm.");
    }

    // sku dùng filtered unique index (chỉ áp dụng khi khác NULL) - xem Database.sql dòng 423.
    // loaiTruVariantId: khi update, không tự so trùng với chính variant đang sửa.
    private async Task KiemTraTrungSkuAsync(List<ProductVariantRequestDto> danhSachVariant, int? loaiTruVariantId)
    {
        var skuGuiLen = danhSachVariant
            .Where(v => !string.IsNullOrWhiteSpace(v.Sku))
            .Select(v => v.Sku!)
            .ToList();

        if (skuGuiLen.Count != skuGuiLen.Distinct().Count())
            throw new InvalidOperationException("Danh sách biến thể có SKU bị trùng nhau.");

        foreach (var sku in skuGuiLen)
        {
            var trung = await _dbContext.ProductVariants
                .AnyAsync(v => v.Sku == sku && (loaiTruVariantId == null || v.VariantId != loaiTruVariantId));
            if (trung)
                throw new InvalidOperationException($"SKU '{sku}' đã tồn tại.");
        }
    }

    // gia_tu chỉ là giá tham khảo để hiển thị (giá thấp nhất từ biến thể đang kinh doanh) - xem SRS mục 7.3
    private static void CapNhatGiaTu(Product product)
    {
        var giaConHieuLuc = product.Variants
            .Where(v => v.DangKinhDoanh)
            .Select(v => (decimal?)v.Gia)
            .ToList();

        product.GiaTu = giaConHieuLuc.Count > 0 ? giaConHieuLuc.Min() : null;
    }

    private static ProductVariantDto MapVariantToDto(ProductVariant v) => new()
    {
        VariantId = v.VariantId,
        TenBienThe = v.TenBienThe,
        Gia = v.Gia,
        SoLuongTon = v.SoLuongTon,
        Sku = v.Sku,
        DangKinhDoanh = v.DangKinhDoanh
    };

    private static ProductDto MapToDto(Product p) => new()
    {
        ProductId = p.ProductId,
        Ten = p.Ten,
        MoTa = p.MoTa,
        CategoryId = p.CategoryId,
        BrandId = p.BrandId,
        LieuLuong = p.LieuLuong,
        GiaTu = p.GiaTu,
        DiemDanhGiaTb = p.DiemDanhGiaTb,
        SoLuongDanhGia = p.Reviews.Count,
        DangKinhDoanh = p.DangKinhDoanh,
        Variants = p.Variants.Select(MapVariantToDto).ToList(),
        Images = p.Images.Select(i => new ProductImageDto
        {
            ImageId = i.ImageId,
            Url = i.Url,
            LaAnhChinh = i.LaAnhChinh
        }).ToList(),
        Conditions = p.ProductConditions.Select(pc => new ConditionDto
        {
            ConditionId = pc.ConditionId,
            TenTinhTrang = pc.Condition.TenTinhTrang
        }).ToList()
    };

    // idsMuon == null: không đổi (Update không gửi field này); rỗng: gỡ hết. Ném lỗi nếu có id
    // không tồn tại trong bảng conditions, cùng kiểu xác thực FK đang dùng cho category_id/brand_id.
    private async Task<List<Condition>> LayDanhSachConditionHopLeAsync(List<int>? idsMuon)
    {
        if (idsMuon == null || idsMuon.Count == 0)
            return new List<Condition>();

        var idsDistinct = idsMuon.Distinct().ToList();
        var conditions = await _dbContext.Conditions
            .Where(c => idsDistinct.Contains(c.ConditionId))
            .ToListAsync();

        if (conditions.Count != idsDistinct.Count)
            throw new InvalidOperationException("Có tình trạng sức khỏe không tồn tại.");

        return conditions;
    }

    public async Task<ProductVariantInfoDto?> LayThongTinBienTheAsync(int variantId)
    {
        var variant = await _dbContext.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(v => v.VariantId == variantId);

        if (variant == null || !variant.DangKinhDoanh || !variant.Product.DangKinhDoanh)
            return null;

        return new ProductVariantInfoDto
        {
            VariantId = variant.VariantId,
            ProductId = variant.ProductId,
            TenSanPham = variant.Product.Ten,
            ThuocTinh = variant.TenBienThe,
            Gia = variant.Gia,
            HinhAnhChinh = variant.Product.Images.FirstOrDefault(i => i.LaAnhChinh)?.Url
                ?? variant.Product.Images.FirstOrDefault()?.Url,
            SoLuongTon = variant.SoLuongTon
        };
    }
}
