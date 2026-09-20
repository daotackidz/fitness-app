namespace FitBodyApp.Application.Common;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
