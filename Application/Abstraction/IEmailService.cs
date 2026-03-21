namespace Application.Abstraction;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
}