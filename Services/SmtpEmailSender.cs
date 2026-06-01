using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CosmeticCompanyMVC.Models;

namespace CosmeticCompanyMVC.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            using var msg = new MailMessage();
            msg.To.Add(new MailAddress(email));
            msg.From = new MailAddress(_settings.FromEmail, _settings.FromName);
            msg.Subject = subject;
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;

            if (string.IsNullOrWhiteSpace(_settings.SmtpHost) || _settings.SmtpPort == 0)
            {
                _logger.LogWarning("SMTP settings not configured. Email to {Email} was not sent. Subject: {Subject}", email, subject);
                return;
            }

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.UseSSL,
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass)
            };

            await client.SendMailAsync(msg);
            _logger.LogInformation("Email sent to {Email} (Subject: {Subject})", email, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
            throw;
        }
    }
}