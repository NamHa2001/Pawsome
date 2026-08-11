namespace Pawsome.API.DTOs.DonHang;

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int VariantId { get; set; }
    public string TenSanPham { get; set; } = null!;
    public string TenBienThe { get; set; } = null!;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien => DonGia * SoLuong;
}