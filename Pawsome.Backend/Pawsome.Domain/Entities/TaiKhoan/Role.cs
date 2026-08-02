namespace Pawsome.Domain.Entities.TaiKhoan;

public class Role
{
    public int RoleId { get; set; }
    public string TenVaiTro { get; set; } = null!;
    public string? MoTa { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
