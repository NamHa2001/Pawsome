using Pawsome.Domain.Entities.SanPham;
using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.BlogQuanTri;

public class Wishlist
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime NgayThem { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
