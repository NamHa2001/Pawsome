using Pawsome.Domain.Entities.BlogQuanTri;
using Pawsome.Domain.Entities.Common;
using Pawsome.Domain.Entities.DonHang;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Domain.Entities.SanPham;

namespace Pawsome.Domain.Entities.TaiKhoan;

public class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string? SoDienThoai { get; set; }
    public int DiemPawpoints { get; set; }
    public string TrangThai { get; set; } = "active";
    public DateTime NgayTao { get; set; }
    public DateTime NgayCapNhat { get; set; }

    public Role Role { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<AutoOrder> AutoOrders { get; set; } = new List<AutoOrder>();
    public ICollection<PawPointsTransaction> PawPointsTransactions { get; set; } = new List<PawPointsTransaction>();
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
