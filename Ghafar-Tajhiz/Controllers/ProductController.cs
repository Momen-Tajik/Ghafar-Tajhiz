using BusinessLogic.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index(int id)
        {
            var product=await _productService.GetProductById(id);
            return View(product);
        }

        public async Task<IActionResult> ProductList(int page=1, int pageSize=4, string search=null)
        {
            var data=await _productService.GetProductPagination(page, pageSize, search);

            ViewBag.CurrentPage = data.Page;
            ViewBag.TotalPages = data.TotalPage;
            ViewBag.Search = search;

            return View(data.Products);
        }
    }
}
