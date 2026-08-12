namespace Pawsome.API.DTOs.DonHang;

public class PawPointsHistoryFilterDto
{
    public string? Loai { get; set; }  // lọc theo earn/redeem/bonus, để trống = lấy hết
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}