using Pawsome.Domain.Entities.GioHang;
using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Domain.Entities.SanPham;

public class ProductVariant
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public string TenBienThe { get; set; } = null!;
    public decimal Gia { get; set; }
    public int SoLuongTon { get; set; }
    public string? Sku { get; set; }
    public bool DangKinhDoanh { get; set; } = true;

    public Product Product { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<AutoOrder> AutoOrders { get; set; } = new List<AutoOrder>();
}
