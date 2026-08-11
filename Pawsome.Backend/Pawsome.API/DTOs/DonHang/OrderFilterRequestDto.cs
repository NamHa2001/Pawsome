namespace Pawsome.API.DTOs.DonHang;

public class OrderFilterRequestDto
{
    public string? TrangThai { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}