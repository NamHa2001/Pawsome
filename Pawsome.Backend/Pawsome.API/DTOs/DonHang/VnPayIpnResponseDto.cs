using System.Text.Json.Serialization;

namespace Pawsome.API.DTOs.DonHang;

public class VnPayIpnResponseDto
{
    [JsonPropertyName("RspCode")]
    public string RspCode { get; set; } = null!;

    [JsonPropertyName("Message")]
    public string Message { get; set; } = null!;
}