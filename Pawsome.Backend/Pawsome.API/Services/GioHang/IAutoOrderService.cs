using Pawsome.API.DTOs.GioHang;

namespace Pawsome.API.Services.GioHang
{
    public interface IAutoOrderService
    {
        Task<IEnumerable<AutoOrderDto>> LayDanhSachAsync(int userId);
        Task<AutoOrderDto> TaoAsync(int userId, CreateAutoOrderDto dto);
        Task<AutoOrderDto> CapNhatAsync(int userId, int id, UpdateAutoOrderDto dto);
        Task TamDungAsync(int userId, int id);
        Task KichHoatAsync(int userId, int id);
        Task HuyAsync(int userId, int id);

        // Dùng bởi AutoOrderProcessingService (BackgroundService chạy định kỳ) - đúng YC-6.1
        // (tự tạo đơn khi tới hạn) và YC-6.3 (nhắc nhở trước khi xử lý). Trả về số lượng đã xử
        // lý được để ghi log/theo dõi.
        Task<int> XuLyDonDenHanAsync();
        Task<int> GuiNhacNhoTruocHanAsync(int soNgayTruoc);
    }
}