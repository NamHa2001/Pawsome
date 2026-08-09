namespace Pawsome.API.DTOs.BlogQuanTri;

public class BlogFilterRequestDto
{
    public string? TuKhoa { get; set; }
    public string? ChuDe { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
