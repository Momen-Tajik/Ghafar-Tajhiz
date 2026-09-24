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

        public CategoriesController(CategoryService categoryService)
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
            [Bind("CategoryName,CategoryDescription")] Category category)
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
            [Bind("CategoryId,CategoryName,CategoryDescription")]
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
                return NotFound();

            var result =
                await _categoryService.DeleteCategory(id);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "دسته‌بندی با موفقیت حذف شد.";

            return RedirectToAction(nameof(Index));
        }
    }
}