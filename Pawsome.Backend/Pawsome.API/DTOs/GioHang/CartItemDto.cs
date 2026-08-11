namespace Pawsome.API.DTOs.GioHang
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int VariantId { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public string ThuocTinh { get; set; } = null!;  
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }
}
