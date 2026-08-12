namespace Pawsome.API.DTOs.DonHang;

public class CreatePaymentResultDto
{
    public int PaymentId { get; set; }
    public string PayUrl { get; set; } = null!;
}