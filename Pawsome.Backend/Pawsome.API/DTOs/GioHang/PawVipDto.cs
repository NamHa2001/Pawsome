namespace Pawsome.API.DTOs.GioHang
{
    public class PawVipStatusDto
    {
        public string? Tier { get; set; }
        public DateOnly? HetHan { get; set; }
    }

    public class CreatePawVipPaymentDto
    {
        public string Tier { get; set; } = null!;
    }

    public class PawVipPaymentStatusDto
    {
        public int PawVipPaymentId { get; set; }
        public string Tier { get; set; } = null!;
        public string TrangThai { get; set; } = null!;
    }
}
