using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.CategoryRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public CategoryRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }
        public async Task Add(Category category)
        {
            _context.Categories.Add(category);
             await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var c = await GetById(id);
            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            var data= await _context.Categories.ToListAsync();
            return data;
        }

        public async Task<Category> GetById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task Update(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
