using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.CategoryServices
{
    public class CategoryService
    {
        private readonly GhafarTajhizShopDbContext _context;

        public CategoryService(GhafarTajhizShopDbContext context)
        {
            _context = context;
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
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == category.CategoryId);

            if (existing == null)
                return false;

            existing.CategoryName = category.CategoryName;
            existing.CategoryDescription = category.CategoryDescription;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return false;

            // اگر Product داشته باشد، FK Restrict از حذف جلوگیری می‌کند.
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}