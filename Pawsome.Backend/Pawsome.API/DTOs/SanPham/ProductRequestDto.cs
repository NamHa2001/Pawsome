using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ProductRequestDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống", AllowEmptyStrings = false)]
    [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
    public string Ten { get; set; } = null!;

    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Danh mục không được để trống")]
    public int CategoryId { get; set; }

    public int? BrandId { get; set; }

    [StringLength(100, ErrorMessage = "Liều lượng không được vượt quá 100 ký tự")]
    public string? LieuLuong { get; set; }

    // Mỗi sản phẩm phải có tối thiểu 1 biến thể (xem Pawsome_Database.sql mục Quy ước) -
    // giỏ hàng/đơn hàng luôn tham chiếu variant_id, không có variant thì không bán được.
    [Required(ErrorMessage = "Sản phẩm phải có ít nhất 1 biến thể")]
    [MinLength(1, ErrorMessage = "Sản phẩm phải có ít nhất 1 biến thể")]
    public List<ProductVariantRequestDto> Variants { get; set; } = new();

    public List<ProductImageRequestDto>? Images { get; set; }
}
