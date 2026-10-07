using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.CategoryServices
{
    public class CategoryService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            GhafarTajhizShopDbContext context,
            ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task CreateCategory(Category category)
        {
            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();
        }


        public async Task<IReadOnlyList<Category>> GetCategories()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }


        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }


        public async Task<bool> EditCategory(Category category)
        {
            var existing =
                await _context.Categories
                    .FirstOrDefaultAsync(c =>
                        c.CategoryId == category.CategoryId);

            if (existing == null)
                return false;

            existing.CategoryName =
                category.CategoryName;

            existing.CategoryDescription =
                category.CategoryDescription;

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<CategoryDeleteResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return CategoryDeleteResult.NotFound;


            var category =
                await _context.Categories
                    .FirstOrDefaultAsync(c =>
                        c.CategoryId == id);

            if (category == null)
                return CategoryDeleteResult.NotFound;


            // اگر این دسته‌بندی محصول داشته باشد،
            // حذف آن مجاز نیست.
            var hasProducts =
                await _context.Products
                    .AnyAsync(p =>
                        p.CategoryId == id);

            if (hasProducts)
                return CategoryDeleteResult.HasProducts;


            _context.Categories.Remove(category);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete category {CategoryId}",
                    id);

                return CategoryDeleteResult.DatabaseError;
            }


            return CategoryDeleteResult.Success;
        }
    }
}