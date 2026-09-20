using System.Net;
using System.Net.Mail;
using FitBodyApp.Application.Common;
using Microsoft.Extensions.Configuration;

namespace FitBodyApp.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var host = _configuration["EmailSettings:Host"]!;
        var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        var username = _configuration["EmailSettings:Username"]!;
        var password = _configuration["EmailSettings:Password"]!;
        var senderName = _configuration["EmailSettings:SenderName"] ?? "FitBody";
        var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? username;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
