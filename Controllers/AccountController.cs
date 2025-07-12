using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Models;
using Zdrav_I_SIlen.Models.ViewModels;
using Zdrav_I_SIlen.Services;

namespace Zdrav_I_SIlen.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;

        public AccountController(
            ApplicationDbContext context,
            ILogger<AccountController> logger,
            IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password);
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == hashedPassword && u.IsActive);

                if (user != null)
                {
                    // Set session or authentication cookie
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.FullName);

                    TempData["Success"] = $"Добре дошли, {user.FirstName}!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Невалиден имейл или парола.");
            }

            return View(model);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Този имейл адрес вече се използва.");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = HashPassword(model.Password),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Регистрацията е успешна! Моля, влезте в профила си.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Update session
                    HttpContext.Session.SetString("UserName", user.FullName);

                    TempData["Success"] = "Профилът е актуализиран успешно.";
                }
            }

            return View(model);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

                if (user != null)
                {
                    var currentPasswordHash = HashPassword(model.CurrentPassword);
                    if (user.Password == currentPasswordHash)
                    {
                        user.Password = HashPassword(model.NewPassword);
                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Паролата е променена успешно.";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "Невалидна текуща парола.");
                    }
                }
            }

            return View(model);
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

                if (user != null)
                {
                    try
                    {
                        // Generate secure reset token
                        var resetToken = GenerateSecureToken();
                        user.PasswordResetCode = resetToken;
                        user.PasswordResetExpiry = DateTime.Now.AddDays(1); // Token expires in 24 hours
                        await _context.SaveChangesAsync();

                        // Create reset link
                        var resetLink = Url.Action("ResetPassword", "Account", 
                            new { token = resetToken }, Request.Scheme);

                        // Send email (simplified for demo)
                        var emailSubject = "Възстановяване на парола - Zdrav I Silen";
                        var emailBody = $@"
                            <h2>Възстановяване на парола</h2>
                            <p>Здравейте {user.FirstName},</p>
                            <p>Получили сте този имейл, защото сте поискали възстановяване на паролата за вашия акаунт.</p>
                            <p>Кликнете върху следния линк, за да възстановите паролата си:</p>
                            <p><a href='{resetLink}'>Възстанови парола</a></p>
                            <p>Този линк е валиден за 24 часа.</p>
                            <p>Ако не сте поискали възстановяване на парола, моля игнорирайте този имейл.</p>
                        ";

                        await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                        TempData["Success"] = "Инструкции за възстановяване на паролата бяха изпратени на вашия имейл.";
                        return RedirectToAction("ForgotPassword");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending password reset email");
                        TempData["Error"] = "Възникна грешка при изпращането на имейла. Моля, опитайте отново.";
                    }
                }

                // Always show success message for security (whether user exists or not)
                TempData["Success"] = "Ако имейлът съществува в системата, ще получите линк за възстановяване на паролата в имейла си.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/ResetPassword
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Невалиден линк за възстановяване на парола.";
                return RedirectToAction("ForgotPassword");
            }

            // Check if token is valid
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetCode == token &&
                                         u.PasswordResetExpiry > DateTime.Now);

            if (user == null)
            {
                TempData["Error"] = "Линкът за възстановяване на парола е невалиден или е изтекъл.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetCode == model.Token &&
                                         u.PasswordResetExpiry > DateTime.Now);

            if (user == null)
            {
                TempData["Error"] = "Линкът за възстановяване на парола е невалиден или е изтекъл.";
                return RedirectToAction("ForgotPassword");
            }

            // Update password
            user.Password = HashPassword(model.NewPassword);
            user.PasswordResetCode = null;
            user.PasswordResetExpiry = null;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Паролата беше успешно възстановена! Можете да влезете с новата парола.";
            return RedirectToAction("Login");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Излязохте успешно от профила си.";
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateSecureToken()
        {
            return Guid.NewGuid().ToString("N"); // Generate secure token without hyphens
        }
    }
} 