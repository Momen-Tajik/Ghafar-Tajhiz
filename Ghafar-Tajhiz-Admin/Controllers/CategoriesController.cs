using BusinessLogic.CategoryServices;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(
            CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories =
                await _categoryService.GetCategories();

            return View(categories);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var category =
                await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CategoryName,CategoryDescription")]
            Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            await _categoryService.CreateCategory(category);

            TempData["Success"] =
                "دسته‌بندی با موفقیت ایجاد شد.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var category =
                await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "CategoryId,CategoryName,CategoryDescription")]
            Category category)
        {
            if (id != category.CategoryId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            var result =
                await _categoryService.EditCategory(category);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "دسته‌بندی با موفقیت بروزرسانی شد.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var category =
                await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] =
                    "شناسه دسته‌بندی نامعتبر است.";

                return RedirectToAction(nameof(Index));
            }


            var result =
                await _categoryService.DeleteCategory(id);


            switch (result)
            {
                case CategoryDeleteResult.Success:

                    TempData["Success"] =
                        "دسته‌بندی با موفقیت حذف شد.";

                    break;


                case CategoryDeleteResult.NotFound:

                    TempData["Error"] =
                        "دسته‌بندی مورد نظر پیدا نشد.";

                    break;


                case CategoryDeleteResult.HasProducts:

                    TempData["Error"] =
                        "این دسته‌بندی دارای محصول است و قابل حذف نیست. " +
                        "ابتدا محصولات این دسته‌بندی را به دسته‌بندی دیگری منتقل کنید.";

                    break;


                case CategoryDeleteResult.DatabaseError:

                    TempData["Error"] =
                        "حذف دسته‌بندی انجام نشد. لطفاً دوباره تلاش کنید.";

                    break;


                default:

                    TempData["Error"] =
                        "خطایی هنگام حذف دسته‌بندی رخ داد.";

                    break;
            }


            return RedirectToAction(nameof(Index));
        }
    }
}