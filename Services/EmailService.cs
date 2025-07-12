using System.Net;
using System.Net.Mail;

namespace Zdrav_I_SIlen.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
        {
            var resetLink = $"{_configuration["EmailSettings:BaseUrl"]}/Account/ResetPassword?token={resetToken}";
            
            string subject = "Възстановяване на парола - Zdrav I Silen";
            string body = $@"
                <html>
                <body>
                    <h2>Здравейте, {userName}!</h2>
                    <p>Получили сте този имейл, защото сте заявили възстановяване на парола за вашия акаунт.</p>
                    <p>Моля, кликнете върху линка по-долу, за да възстановите паролата си:</p>
                    <p><a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Възстанови парола</a></p>
                    <p>Или копирайте и поставете този линк в браузъра си:</p>
                    <p>{resetLink}</p>
                    <p><strong>Този линк ще бъде валиден само 24 часа.</strong></p>
                    <p>Ако не сте заявили възстановяване на парола, моля игнорирайте този имейл.</p>
                    <hr>
                    <p>С уважение,<br>Екипът на Zdrav I Silen</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Validate email address
                if (string.IsNullOrWhiteSpace(toEmail) || !IsValidEmail(toEmail))
                {
                    throw new ArgumentException("Invalid email address", nameof(toEmail));
                }

                var smtpSettings = _configuration.GetSection("EmailSettings");
                
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];
                var smtpServer = smtpSettings["SmtpServer"];
                var smtpPort = smtpSettings["SmtpPort"];
                var enableSsl = smtpSettings["EnableSsl"];
                var fromPassword = smtpSettings["FromPassword"];

                // Use mock mode only if configuration is missing or incomplete
                if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromPassword) || 
                    fromPassword == "your-app-password" || fromPassword == "PUT_YOUR_APP_PASSWORD_HERE")
                {
                    _logger.LogInformation($"""
                        📧 MOCK EMAIL (Development Mode - No valid email config):
                        To: {toEmail}
                        Subject: {subject}
                        Body: {body}
                        """);
                    
                    // Simulate email sending delay
                    await Task.Delay(1000);
                    return;
                }

                // Real email sending
                var fromAddress = new MailAddress(fromEmail, fromName ?? "Zdrav I Silen");
                var toAddress = new MailAddress(toEmail);
                
                using var smtp = new SmtpClient
                {
                    Host = smtpServer ?? "smtp.gmail.com",
                    Port = int.Parse(smtpPort ?? "587"),
                    EnableSsl = bool.Parse(enableSsl ?? "true"),
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    Timeout = 30000 // 30 seconds timeout
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(message);
                _logger.LogInformation($"✅ Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send email to {toEmail}");
                throw;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
} 