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
    }
}