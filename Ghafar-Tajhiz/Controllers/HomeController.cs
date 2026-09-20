using BusinessLogic.ProductServices;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ghafar_Tajhiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;

        public HomeController(
            ILogger<HomeController> logger,
            ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsWithCategory();

            return View(products);
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }
    }
}