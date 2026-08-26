namespace Pawsome.API.Services.SanPham;

public interface IRecaptchaService
{
    Task<bool> VerifyAsync(string token);
}
