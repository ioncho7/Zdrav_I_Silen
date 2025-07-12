namespace Zdrav_I_SIlen.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetCode);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
    }
} 