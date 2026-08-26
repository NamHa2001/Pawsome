using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class SubscribeRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string RecaptchaToken { get; set; } = null!;
}
