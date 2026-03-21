using Application.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        _client = new SendGridClient(apiKey);
        _fromEmail = configuration["SendGrid:FromEmail"];
        _fromName = configuration["SendGrid:FromName"];
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_fromEmail, _fromName),
            Subject = subject,
            HtmlContent = htmlBody
        };
        msg.AddTo(new EmailAddress(to));

        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            _logger.LogError("SendGrid email failed: {StatusCode} - {Error}",
                response.StatusCode, error);
            throw new InvalidOperationException($"Email sending failed: {error}");
        }

        _logger.LogInformation("SendGrid email sent to {Email}", to);
    }
}