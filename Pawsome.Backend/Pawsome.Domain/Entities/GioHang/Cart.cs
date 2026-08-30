using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.GioHang;

public class Cart
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public int? CouponId { get; set; }
    public DateTime NgayCapNhat { get; set; }

    public User User { get; set; } = null!;
    public Coupon? Coupon { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
