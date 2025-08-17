using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Models;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ECommerceWebApp.Controllers
{
    public class SellerController : Controller
    {
        // Dummy database list (baad me DB se replace karenge)

        public static List<Product> products = new List<Product>();
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Seller()
        {
            var user = await _userManager.GetUserAsync(User);
            var sellerId = user.Id;
            var products = _context.Products
                           .Where(p => p.SellerId == sellerId)
                           .ToList();

            return View(products);

        }


        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = new List<string> { "All Categories", "Electronics", "Fashion", "Home" , "Grocery"};
            return View();
        }


        //[HttpPost]
        //public IActionResult Add(Product product)
        //{
        //    products.Add(product);
        //    return RedirectToAction("Seller", "Seller");
        //}

        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Seller");
        }

        //[HttpPost]
        //public IActionResult Add(Product product, IFormFile ImageFile)
        //{
        //    if (ImageFile != null && ImageFile.Length > 0)
        //    {
        //        // Image ka filename aur path set karna
        //        var fileName = Path.GetFileName(ImageFile.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        //        // Image ko server me save karna
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            ImageFile.CopyTo(stream);
        //        }

        //        // Product me image path save karna
        //        product.ImagePath = "/images/" + fileName;
        //    }

        //    // Product ko list ya database me add karna
        //    products.Add(product); // agar database use kar rahi ho to _context.Products.Add(product); _context.SaveChanges();

        //    // Add ke baad wapas Index page pe redirect
        //                 return RedirectToAction("Seller", "Seller");

        //}
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile ImageFile, ApplicationUser model)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var filePath = Path.Combine(imagesFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImagePath = "/images/" + fileName;
            }

            // ✅ SellerId backend se set karo (UI par dikhane ki zarurat nahi)
            var user = await _userManager.GetUserAsync(User);
            product.SellerId = user.Id;
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Seller" , "Seller");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.Categories = new List<string> { "All Categories", "Electronics", "Fashion", "Home", "Grocery" };
            return View(product);
        }


        [HttpPost]
        public IActionResult Edit(Product model, IFormFile ImageFile)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == model.Id);
            if (product == null) return NotFound();

            // Update fields
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Category = model.Category;

            // Agar new image upload ki ho to save karo
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var filePath = Path.Combine(imagesFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImagePath = "/images/" + fileName;
            }

            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Seller", "Seller");
        }


    }
}
