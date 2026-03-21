using Application.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;

namespace Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly string _fromEmail;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IConfiguration configuration, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _fromEmail = configuration["Resend:FromEmail"]!;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = to,
            Subject = subject,
            HtmlBody = htmlBody
        };

        try
        {
            var response = await _resend.EmailSendAsync(message);
            _logger.LogInformation("Resend email sent to {To}. MessageId: {MessageId}", to, response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend email failed to {To}", to);
            throw;
        }
    }
}