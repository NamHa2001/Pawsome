namespace Pawsome.API.DTOs.GioHang
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public string? MaCouponDangApDung { get; set; }
        public decimal GiamGia { get; set; }
        public decimal PhiVanChuyenTamTinh { get; set; }

        public decimal TienHang => Items.Sum(i => i.ThanhTien);
        public decimal TongTien => TienHang - GiamGia + PhiVanChuyenTamTinh;
    }
}
