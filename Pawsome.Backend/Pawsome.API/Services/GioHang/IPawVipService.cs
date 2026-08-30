namespace Pawsome.API.Services.GioHang
{
    public interface IPawVipService
    {
        Task<(string? Tier, DateOnly? HetHan)> LayTrangThaiAsync(int userId);
        Task<string> KichHoatAsync(int userId, string tier);
    }
}
