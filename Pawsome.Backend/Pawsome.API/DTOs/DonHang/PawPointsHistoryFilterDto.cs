namespace Pawsome.API.DTOs.DonHang;

public class PawPointsHistoryFilterDto
{
    public string? Loai { get; set; }  
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}