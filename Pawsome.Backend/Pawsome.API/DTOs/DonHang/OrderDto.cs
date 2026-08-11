namespace Pawsome.API.DTOs.DonHang;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public int? CouponId { get; set; }
    public DateTime NgayDat { get; set; }
    public decimal TienHang { get; set; }
    public decimal PhiVanChuyen { get; set; }
    public decimal GiamGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string TrangThai { get; set; } = null!;
    public string? DonViVanChuyen { get; set; }
    public string? MaVanDon { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}