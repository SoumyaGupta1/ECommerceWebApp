using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Models;

using System.Linq;
using ECommerceWebApp.Areas.Identity.Data;

public class AdminController : Controller
{
    private readonly ApplicationDBContext _context;

    public AdminController(ApplicationDBContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var totalUsers = _context.Users.Count();

        var totalProducts = _context.Products.Count();

        var totalEarnings = _context.Products.Sum(p => p.Price * p.Stock);

        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.TotalEarnings = totalEarnings;

        return View();
    }

    public IActionResult ManageUsers(string roleFilter)
    {
        var users = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(roleFilter))
        {
            users = users.Where(u => u.Role == roleFilter);
        }

        return View(users.ToList());
    }

    // --- Step 2: Edit User ---
    public IActionResult EditUser(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(ApplicationUser model)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
        if (user == null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;

        _context.SaveChanges();
        return RedirectToAction("ManageUsers");
    }

    public IActionResult DeleteUser(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction("ManageUsers");
    }
    // --- PRODUCT LIST ---
    public IActionResult ManageProducts(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, int? rating)
    {
        var products = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = products.Where(p => p.Name.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
        }

        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        if (rating.HasValue)
        {
            products = products.Where(p => p.Rating >= rating.Value);
        }

        return View(products.ToList());
    }


    // --- CREATE PRODUCT ---
    public IActionResult CreateProduct()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateProduct(Product model, IFormFile ImageFile)
    {
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            model.ImagePath = "/images/" + fileName;
        }

        _context.Products.Add(model);
        _context.SaveChanges();
        return RedirectToAction("ManageProducts");
    }


    // --- EDIT PRODUCT ---
    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public IActionResult EditProduct(Product model, IFormFile ImageFile)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == model.Id);
        if (product == null) return NotFound();

       
            product.Name = model.Name;
            product.Price = model.Price;
        product.Category = model.Category;


        // Image Upload Logic
        if (ImageFile != null && ImageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImagePath = "/images/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("ManageProducts");
        

       
    }


    // --- DELETE PRODUCT ---
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("ManageProducts");
    }

}
