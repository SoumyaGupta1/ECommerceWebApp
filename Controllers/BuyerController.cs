using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Models;
using ECommerceWebApp.Areas.Identity.Data;

namespace ECommerceWebApp.Controllers
{
    public class BuyerController : Controller
    {
        // Temporary reference to seller products
        private static List<Product> products = SellerController.products; // ya DB use karo

        // Show products to buyer
        private readonly ApplicationDBContext _context;

        public BuyerController(ApplicationDBContext context)
        {
            _context = context;
        }
        public IActionResult Buyer(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, int? rating)
        {
            var products = _context.Products.AsQueryable();

            ViewBag.Categories = new List<string> { "All Categories", "Electronics", "Fashion", "Home", "Grocery" };

            if (!string.IsNullOrEmpty(searchTerm))
                products = products.Where(p => p.Name.Contains(searchTerm));

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            if (rating.HasValue)
                products = products.Where(p => p.Rating >= rating.Value);

            return View(products.ToList());
        }



        private static List<Product> cart = new List<Product>();

        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                cart.Add(product);
            }
            return RedirectToAction("Cart" , "Buyer");
        }

        public IActionResult Cart()
        {
            return View(cart);
        }

        public IActionResult BuyNow(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
            }
            return View(product);
        }
        [HttpPost]
public IActionResult ConfirmPurchase(int id, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity > 0 && product.Stock >= quantity)
            {
                product.Stock -= quantity;   // ab ye error nahi dega
                _context.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Not enough stock available.";
            }

            return View("ConfirmPurchase", product);
        }

        [HttpPost]
        public IActionResult SubmitRating(int id, int rating)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Rating save karo (simple case: overwrite)
            product.Rating = rating;
            _context.SaveChanges();

            TempData["Message"] = "Thank you! Your rating has been submitted.";

            return RedirectToAction("Buyer", "Buyer"); // wapas product list pe bhej do
        }


    }
}
