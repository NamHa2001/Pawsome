using Pawsome.Domain.Entities.DonHang;

namespace Pawsome.Domain.Entities.TaiKhoan;

public class Address
{
    public int AddressId { get; set; }
    public int UserId { get; set; }
    public string NguoiNhan { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string DiaChiChiTiet { get; set; } = null!;
    public string? PhuongXa { get; set; }
    public string? QuanHuyen { get; set; }
    public string TinhThanh { get; set; } = null!;
    public bool LaMacDinh { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
