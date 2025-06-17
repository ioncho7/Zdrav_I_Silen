using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zdrav_I_SIlen.Models;
using Zdrav_I_SIlen.Models.ViewModels;
using Zdrav_I_SIlen.Data;

namespace Zdrav_I_SIlen.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

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
