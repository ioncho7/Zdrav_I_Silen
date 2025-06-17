using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Models;
using Zdrav_I_SIlen.Models.ViewModels;

namespace Zdrav_I_SIlen.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
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

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = user.PhoneNumber;

            return View();
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            _context.Update(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserName", user.FullName);
            TempData["Success"] = "Профилът е обновен успешно!";

            return RedirectToAction("Profile");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Новите пароли не съвпадат.";
                return RedirectToAction("Profile");
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var hashedCurrentPassword = HashPassword(currentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                TempData["Error"] = "Текущата парола е неправилна.";
                return RedirectToAction("Profile");
            }

            user.Password = HashPassword(newPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Паролата е променена успешно!";
            return RedirectToAction("Profile");
        }

        // POST: Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Излязохте успешно от профила си.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
} 