namespace Pawsome.API.DTOs.TaiKhoan;

public class AddressDto
{
    public int AddressId { get; set; }
    public string NguoiNhan { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string DiaChiChiTiet { get; set; } = null!;
    public string? PhuongXa { get; set; }
    public string? QuanHuyen { get; set; }
    public string TinhThanh { get; set; } = null!;
    public bool LaMacDinh { get; set; }
}