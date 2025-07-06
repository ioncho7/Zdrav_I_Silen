using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Models;

namespace Zdrav_I_SIlen.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            try
            {
                // For now, we'll get all cart items
                // In a real application, you'd filter by user session or user ID
                var cartItems = await _context.Carts.ToListAsync();
                return View(cartItems);
            }
            catch (Exception ex)
            {
                // Log the error here in production
                TempData["Error"] = "Възникна грешка при зареждане на количката.";
                return View(new List<Cart>());
            }
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.ProductId == productId);

                if (existingCartItem != null)
                {
                    // Update quantity
                    existingCartItem.Quantity += quantity;
                    _context.Update(existingCartItem);
                }
                else
                {
                    // Add new cart item
                    var cartItem = new Cart
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Name = product.Name,
                        Price = product.UnitPrice,
                        Quantity = quantity,
                        Image = product.ImagePath
                    };

                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Продуктът е добавен в количката!" });
                }

                TempData["Success"] = "Продуктът е добавен в количката!";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                // Log the error here in production
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Възникна грешка при добавяне на продукта." });
                }

                TempData["Error"] = "Възникна грешка при добавяне на продукта.";
                return RedirectToAction("Index", "Products");
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid cartId, int quantity)
        {
            try
            {
                var cartItem = await _context.Carts.FindAsync(cartId);
                if (cartItem == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Продуктът не е намерен в количката." });
                    }
                    return NotFound();
                }

                if (quantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    _context.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error here in production
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Възникна грешка при актуализиране на количеството." });
                }

                TempData["Error"] = "Възникна грешка при актуализиране на количеството.";
                return RedirectToAction("Index");
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid cartId)
        {
            try
            {
                var cartItem = await _context.Carts.FindAsync(cartId);
                if (cartItem == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Продуктът не е намерен в количката." });
                    }
                    return NotFound();
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                TempData["Success"] = "Продуктът е премахнат от количката!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error here in production
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Възникна грешка при премахване на продукта." });
                }

                TempData["Error"] = "Възникна грешка при премахване на продукта.";
                return RedirectToAction("Index");
            }
        }

        // POST: Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var cartItems = await _context.Carts.ToListAsync();
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                TempData["Success"] = "Количката е изчистена!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error here in production
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Възникна грешка при изчистване на количката." });
                }

                TempData["Error"] = "Възникна грешка при изчистване на количката.";
                return RedirectToAction("Index");
            }
        }

        // GET: Cart/GetCartCount
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var count = await _context.Carts.SumAsync(c => c.Quantity);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                // Log the error here in production
                return Json(new { count = 0 });
            }
        }
    }
} 