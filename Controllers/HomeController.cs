using System.Diagnostics;
using ECommerceWebApp.Areas.Identity.Data;
using ECommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly ApplicationDBContext _context;

        public HomeController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            return View(products.ToList());
        }

        public IActionResult Products(string searchTerm, decimal? minPrice, decimal? maxPrice, int? rating)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                //products = products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
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


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult seller()
        {
            return View();
        }
        public IActionResult Buyer()
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
