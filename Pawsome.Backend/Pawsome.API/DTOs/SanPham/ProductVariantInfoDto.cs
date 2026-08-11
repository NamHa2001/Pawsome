namespace Pawsome.API.Services.SanPham;

// DTO rút gọn, CHỈ dùng để các Phần khác (3, 4, 5) hiển thị thông tin biến thể sản phẩm
// (tên sản phẩm, ảnh, thuộc tính biến thể, giá) mà không cần load ProductDto đầy đủ của Phần 2.
public class ProductVariantInfoDto
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public string TenSanPham { get; set; } = null!;
    public string ThuocTinh { get; set; } = null!;  
    public decimal Gia { get; set; }
    public string? HinhAnhChinh { get; set; }
    public int SoLuongTon { get; set; }
}