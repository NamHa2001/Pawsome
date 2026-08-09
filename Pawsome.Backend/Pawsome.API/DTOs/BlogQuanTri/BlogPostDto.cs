namespace Pawsome.API.DTOs.BlogQuanTri;

public class BlogPostDto
{
    public int PostId { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public string? ChuDe { get; set; }
    public int TacGiaId { get; set; }
    public string TenTacGia { get; set; } = null!;
    public string? AnhDaiDien { get; set; }
    public DateTime NgayDang { get; set; }
}
