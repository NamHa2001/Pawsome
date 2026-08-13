namespace Pawsome.API.Common.Email;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body);
}