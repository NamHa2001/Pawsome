using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class CreateAddressRequestDto
{
    [Required(ErrorMessage = "Tên người nhận không được để trống")]
    public string NguoiNhan { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    public string SoDienThoai { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ chi tiết không được để trống")]
    public string DiaChiChiTiet { get; set; } = null!;

    public string? PhuongXa { get; set; }
    public string? QuanHuyen { get; set; }

    [Required(ErrorMessage = "Tỉnh/Thành phố không được để trống")]
    public string TinhThanh { get; set; } = null!;

    public bool LaMacDinh { get; set; } = false;
}