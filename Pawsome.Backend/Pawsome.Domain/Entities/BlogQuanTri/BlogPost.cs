using Pawsome.Domain.Entities.TaiKhoan;

namespace Pawsome.Domain.Entities.BlogQuanTri;

public class BlogPost
{
    public int PostId { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public string? ChuDe { get; set; }
    public int TacGiaId { get; set; }
    public string? AnhDaiDien { get; set; }
    public DateTime NgayDang { get; set; }

    public User TacGia { get; set; } = null!;
}
