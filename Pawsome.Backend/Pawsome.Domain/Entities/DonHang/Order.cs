using Pawsome.Domain.Entities.GioHang;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.DonHang;

public class Order
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
    public DateTime NgayCapNhat { get; set; }

    public User User { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<PawPointsTransaction> PawPointsTransactions { get; set; } = new List<PawPointsTransaction>();
}
