using BusinessLogic.BasketServices;
using BusinessLogic.CommentServices;
using BusinessLogic.ProductServices;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CommentService _commentService;
        public ProductController(ProductService productService, CommentService commentService)
        {
            _productService = productService;
            _commentService = commentService;
        }
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
                return BadRequest();

            var product = await _productService.GetProductById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> ProductList(int page = 1, int pageSize = 8, string search = null, string sort = "newest")
        {
            var data = await _productService.GetProductPagination(page, pageSize, search, sort);

            ViewBag.CurrentPage = data.Page;
            ViewBag.TotalPages = data.TotalPage;
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(data.Products);
        }
        public async Task<IActionResult> GetProduct(int id)
        {
            
            var product = await _productService.GetProductById(id);
            
            if (product == null)
                return NotFound();

            return PartialView(product);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProductComment(AddCommentDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
             await _commentService.CreateComment(model.text,model.productId,model.userName);

            return RedirectToAction("Index","Home");

        }
    }
}
