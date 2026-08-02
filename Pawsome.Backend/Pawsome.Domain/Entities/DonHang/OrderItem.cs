using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Domain.Entities.DonHang;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int VariantId { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }

    public Order Order { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
