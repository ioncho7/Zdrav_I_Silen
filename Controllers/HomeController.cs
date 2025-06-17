using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zdrav_I_SIlen.Models;
using Zdrav_I_SIlen.Models.ViewModels;
using Zdrav_I_SIlen.Data;

namespace Zdrav_I_SIlen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Get featured products for homepage
            var featuredProducts = _context.Products
                .Take(3)
                .ToList();
            
            return View(featuredProducts);
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
