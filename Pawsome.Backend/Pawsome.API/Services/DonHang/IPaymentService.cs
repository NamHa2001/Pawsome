using Pawsome.API.DTOs.DonHang;

namespace Pawsome.API.Services.DonHang;
public interface IPaymentService
{
    Task<CreatePaymentResultDto> CreateMoMoPaymentAsync(int userId, int orderId);
    Task HandleMoMoIpnAsync(MoMoIpnRequestDto dto);

    Task<CreatePaymentResultDto> CreateVnPayPaymentAsync(int userId, int orderId, string clientIp);
    Task<bool> HandleVnPayIpnAsync(Dictionary<string, string> query);

    Task<PaymentDto?> GetByOrderIdAsync(int userId, int orderId);
}