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
            // For now, we'll get all cart items
            // In a real application, you'd filter by user session or user ID
            var cartItems = await _context.Carts.ToListAsync();
            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
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
            return RedirectToAction("Index", "Products");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid cartId, int quantity)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
            {
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

        // POST: Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(Guid cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
            {
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

        // POST: Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var cartItems = await _context.Carts.ToListAsync();
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Количката е изчистена!";
            return RedirectToAction("Index");
        }

        // GET: Cart/GetCartCount
        public async Task<IActionResult> GetCartCount()
        {
            var count = await _context.Carts.SumAsync(c => c.Quantity);
            return Json(new { count });
        }
    }
} 