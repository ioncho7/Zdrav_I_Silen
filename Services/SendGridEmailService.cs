using SendGrid;
using SendGrid.Helpers.Mail;

namespace Zdrav_I_SIlen.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly IConfiguration _configuration;

        public SendGridEmailService(
            ISendGridClient sendGridClient, 
            ILogger<SendGridEmailService> logger,
            IConfiguration configuration)
        {
            _sendGridClient = sendGridClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetCode)
        {
            try
            {
                var fromEmail = _configuration["SendGrid:FromEmail"];
                var fromName = _configuration["SendGrid:FromName"];

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail, userName);
                var subject = "Възстановяване на парола - Здрав и Силен";

                // Create HTML content
                var htmlContent = CreatePasswordResetHtmlContent(userName, resetCode);
                
                // Create plain text content
                var plainTextContent = CreatePasswordResetPlainTextContent(userName, resetCode);

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to send password reset email to {Email}. Status: {StatusCode}", 
                        toEmail, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var fromEmail = _configuration["SendGrid:FromEmail"];
                var fromName = _configuration["SendGrid:FromName"];

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail, userName);
                var subject = "Добре дошли в Здрав и Силен!";

                // Create HTML content
                var htmlContent = CreateWelcomeHtmlContent(userName);
                
                // Create plain text content
                var plainTextContent = CreateWelcomePlainTextContent(userName);

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Welcome email sent successfully to {Email}", toEmail);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to send welcome email to {Email}. Status: {StatusCode}", 
                        toEmail, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email to {Email}", toEmail);
                return false;
            }
        }

        private string CreatePasswordResetHtmlContent(string userName, string resetCode)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Възстановяване на парола</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .code {{ font-size: 24px; font-weight: bold; color: #007bff; text-align: center; 
                 padding: 15px; background-color: #e9ecef; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Здрав и Силен</h1>
        </div>
        <div class='content'>
            <h2>Здравейте, {userName}!</h2>
            <p>Получихме заявка за възстановяване на паролата за вашия акаунт.</p>
            <p>Вашият код за възстановяване е:</p>
            <div class='code'>{resetCode}</div>
            <p>Този код е валиден в продължение на 24 часа.</p>
            <p>Ако не сте заявили възстановяване на парола, моля игнорирайте този имейл.</p>
            <p>За въпроси се свържете с нас на: info@zdravisilen.bg</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 Здрав и Силен. Всички права запазени.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string CreatePasswordResetPlainTextContent(string userName, string resetCode)
        {
            return $@"
Здравейте, {userName}!

Получихме заявка за възстановяване на паролата за вашия акаунт.

Вашият код за възстановяване е: {resetCode}

Този код е валиден в продължение на 24 часа.

Ако не сте заявили възстановяване на парола, моля игнорирайте този имейл.

За въпроси се свържете с нас на: info@zdravisilen.bg

С уважение,
Екипът на Здрав и Силен
";
        }

        private string CreateWelcomeHtmlContent(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Добре дошли</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Добре дошли в Здрав и Силен!</h1>
        </div>
        <div class='content'>
            <h2>Здравейте, {userName}!</h2>
            <p>Благодарим ви, че се регистрирахте в нашия онлайн магазин!</p>
            <p>Вече можете да разглеждате и поръчвате нашите продукти за здравословен начин на живот.</p>
            <p>Ако имате въпроси, не се колебайте да се свържете с нас на: info@zdravisilen.bg</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 Здрав и Силен. Всички права запазени.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string CreateWelcomePlainTextContent(string userName)
        {
            return $@"
Здравейте, {userName}!

Благодарим ви, че се регистрирахте в нашия онлайн магазин!

Вече можете да разглеждате и поръчвате нашите продукти за здравословен начин на живот.

Ако имате въпроси, не се колебайте да се свържете с нас на: info@zdravisilen.bg

С уважение,
Екипът на Здрав и Силен
";
        }
    }
} 