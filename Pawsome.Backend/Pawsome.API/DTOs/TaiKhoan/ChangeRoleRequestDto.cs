using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.TaiKhoan;

public class ChangeRoleRequestDto
{
    [Required(ErrorMessage = "Vai trò mới không được để trống")]
    public int RoleId { get; set; }
}