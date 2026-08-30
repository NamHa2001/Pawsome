using Pawsome.API.DTOs.DonHang;
using Pawsome.API.DTOs.GioHang;

namespace Pawsome.API.Services.GioHang
{
    public interface IPawVipPaymentService
    {
        Task<CreatePaymentResultDto> CreateMoMoPaymentAsync(int userId, string tier);
        Task HandleMoMoIpnAsync(MoMoIpnRequestDto dto);
        Task<CreatePaymentResultDto> CreateVnPayPaymentAsync(int userId, string tier, string clientIp);
        Task<bool> HandleVnPayIpnAsync(Dictionary<string, string> query);
        Task<PawVipPaymentStatusDto?> GetStatusAsync(int userId, int pawVipPaymentId);
    }
}
