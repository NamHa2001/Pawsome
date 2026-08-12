namespace Pawsome.API.DTOs.DonHang;

public class MoMoIpnRequestDto
{
    public string PartnerCode { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string RequestId { get; set; } = null!;
    public long Amount { get; set; }
    public string OrderInfo { get; set; } = null!;
    public string OrderType { get; set; } = null!;
    public string TransId { get; set; } = null!;
    public int ResultCode { get; set; }
    public string Message { get; set; } = null!;
    public string PayType { get; set; } = null!;
    public long ResponseTime { get; set; }
    public string ExtraData { get; set; } = "";
    public string Signature { get; set; } = null!;
}