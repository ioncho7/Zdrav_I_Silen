using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zdrav_I_SIlen.Models;
using Zdrav_I_SIlen.Models.ViewModels;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Services;

namespace Zdrav_I_SIlen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get featured products for homepage - using async to prevent blocking
                var featuredProducts = await _context.Products
                    .Take(3)
                    .ToListAsync();
                
                return View(featuredProducts);
            }
            catch (Exception ex)
            {
                // Log the error (you should implement proper logging)
                Console.WriteLine($"Database error: {ex.Message}");
                
                // Return empty list if database is not available
                return View(new List<Product>());
            }
        }

        // Test database connection
        public async Task<IActionResult> TestDb()
        {
            try
            {
                // Test if we can connect to the database
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    var productCount = await _context.Products.CountAsync();
                    var categoryCount = await _context.Categories.CountAsync();
                    
                    ViewBag.Message = $"✅ Database connection successful! Products: {productCount}, Categories: {categoryCount}";
                }
                else
                {
                    ViewBag.Message = "❌ Cannot connect to database";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"❌ Database error: {ex.Message}";
            }
            
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Here you would typically send an email or save to database
                // For now, we'll just show a success message
                TempData["Success"] = "Вашето съобщение беше изпратено успешно!";
                return RedirectToAction("Contact");
            }

            return View(model);
        }

        // Test email functionality (remove in production)
        public async Task<IActionResult> TestEmail(string email = "test@example.com")
        {
            try
            {
                await _emailService.SendEmailAsync(
                    email,
                    "Test Email - Zdrav I Silen",
                    "<h2>Test Email</h2><p>This is a test email to verify your email configuration is working correctly.</p><p>If you receive this, your email service is properly configured!</p>"
                );
                
                ViewBag.Message = $"✅ Test email sent successfully to {email}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"❌ Failed to send email: {ex.Message}";
            }
            
            return View("TestDb"); // Reuse the TestDb view for display
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
