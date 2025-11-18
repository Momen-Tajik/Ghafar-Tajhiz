using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public ProductRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task Add(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var p = await GetById(id);
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Product> GetAll(Expression<Func<Product, bool>> where = null)
        {
            var products = _context.Products.AsQueryable();
            if (where != null) 
            {
                products = products.Where(where);
            }
            return products;
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task Update(Product category)
        {
            _context.Products.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
