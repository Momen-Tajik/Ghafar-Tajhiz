using BusinessLogic.CategoryServices;
using BusinessLogic.ProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductsController(
            ProductService productService,
            CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products =
                await _productService.GetProductsWithCategory();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var products =
                await _productService.GetProductsWithCategory();

            var product =
                products.FirstOrDefault(p =>
                    p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories(productDto.CategoryId);
                return View(productDto);
            }

            await _productService.CreateProduct(productDto);

            TempData["Success"] =
                "محصول با موفقیت ایجاد شد.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var product =
                await _productService.GetProductDtoById(id);

            if (product == null)
                return NotFound();

            await LoadCategories(product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ProductDto product)
        {
            if (id != product.ProductId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadCategories(product.CategoryId);
                return View(product);
            }

            var result =
                await _productService.UpdateProduct(product);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "محصول با موفقیت بروزرسانی شد.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var products =
                await _productService.GetProductsWithCategory();

            var product =
                products.FirstOrDefault(p =>
                    p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int productId)
        {
            if (productId <= 0)
                return NotFound();

            var result =
                await _productService.DeleteProduct(productId);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "محصول با موفقیت حذف شد.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(
            int? selectedCategoryId = null)
        {
            var categories =
                await _categoryService.GetCategories();

            ViewData["CategoryId"] =
                new SelectList(
                    categories,
                    "CategoryId",
                    "CategoryName",
                    selectedCategoryId);
        }
    }
}