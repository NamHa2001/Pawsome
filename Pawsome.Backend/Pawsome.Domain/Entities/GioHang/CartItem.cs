using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Domain.Entities.GioHang;

public class CartItem
{
    public int CartItemId { get; set; }
    public int CartId { get; set; }
    public int VariantId { get; set; }
    public int SoLuong { get; set; } = 1;

    public Cart Cart { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
