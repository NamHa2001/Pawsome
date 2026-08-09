using Pawsome.API.Common;
using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

// Sản phẩm, biến thể, hình ảnh, tìm kiếm/lọc, + hàm TruTonKho/HoanKho dùng chung
// (Phần 4 gọi khi xử lý đơn hàng - xem Pawsome_KhungDuAn.md mục 5.1)
public interface IProductService
{
    Task<PagedResult<ProductDto>> SearchAsync(ProductFilterRequestDto filter);
    Task<List<ProductSuggestionDto>> GetSuggestionsAsync(string tuKhoa, int soLuong);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductRequestDto dto);
    Task<ProductDto> UpdateAsync(int id, ProductUpdateRequestDto dto);
    Task DeleteAsync(int id);

    Task<ProductVariantDto> AddVariantAsync(int productId, ProductVariantRequestDto dto);
    Task<ProductVariantDto> UpdateVariantAsync(int productId, int variantId, ProductVariantRequestDto dto);
    Task DeleteVariantAsync(int productId, int variantId);

    Task<ProductImageDto> AddImageAsync(int productId, ProductImageRequestDto dto);
    Task DeleteImageAsync(int productId, int imageId);

    // Dùng chung cho Phần 4 (Đơn hàng) gọi khi tạo/hủy đơn - không ghi trực tiếp DB ở Phần khác.
    // Cả 2 hàm ghi thẳng xuống DB bằng UPDATE nguyên tử (ExecuteUpdateAsync), KHÔNG cập nhật lại
    // property trên entity ProductVariant nào đang được track sẵn trong cùng DbContext (nếu trước
    // đó đã đọc variant này bằng FindAsync/Include để lấy giá...). Nếu sau khi gọi cần biết số
    // lượng tồn kho mới, phải query lại (không dùng entity/biến đã đọc từ trước, nó vẫn giữ số cũ).
    Task TruTonKhoAsync(int variantId, int soLuong);
    Task HoanKhoAsync(int variantId, int soLuong);
}
