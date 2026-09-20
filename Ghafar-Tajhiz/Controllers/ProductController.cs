using BusinessLogic.CommentServices;
using BusinessLogic.ProductServices;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CommentService _commentService;

        public ProductController(
            ProductService productService,
            CommentService commentService)
        {
            _productService = productService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
                return BadRequest();

            var product =
                await _productService.GetProductById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> ProductList(
            int page = 1,
            int pageSize = 8,
            string? search = null,
            string sort = "newest")
        {
            var data =
                await _productService.GetProductPagination(
                    page,
                    pageSize,
                    search,
                    sort);

            ViewBag.CurrentPage = data.Page;
            ViewBag.TotalPages = data.TotalPage;
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(data.Products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (id <= 0)
                return BadRequest();

            var product =
                await _productService.GetProductById(id);

            if (product == null)
                return NotFound();

            return PartialView(product);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductComment(
            [FromBody] AddCommentDto model)
        {
            if (model == null ||
                model.ProductId <= 0 ||
                string.IsNullOrWhiteSpace(model.Text))
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "اطلاعات نامعتبر است."
                });
            }

            var claim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(claim, out var userId))
            {
                return Unauthorized(new
                {
                    res = false,
                    msg = "لطفاً ابتدا وارد حساب کاربری شوید."
                });
            }

            var result =
                await _commentService.CreateComment(
                    model.Text,
                    model.ProductId,
                    userId);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "ثبت نظر انجام نشد."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "نظر شما با موفقیت ثبت شد."
            });
        }
    }
}