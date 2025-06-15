using System.Net;
using System.Net.Mail;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlogApp.Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value; 
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            throw new ArgumentException("Recipient email cannot be empty", nameof(toEmail));
        }

        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword),
                EnableSsl = true,
                Timeout = 30000 // 30 seconds
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(
                    _emailSettings.SmtpSenderEmail,
                    _emailSettings.SmtpSenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mail.To.Add(new MailAddress(toEmail));

            _logger.LogInformation("Sending email to {Email}", toEmail);
            await client.SendMailAsync(mail);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw new ApplicationException($"Failed to send email to {toEmail}", ex);
        }
    }
}