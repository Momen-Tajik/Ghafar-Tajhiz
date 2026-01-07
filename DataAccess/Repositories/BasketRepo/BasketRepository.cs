using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BasketRepo
{
    public class BasketRepository:IBasketRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task Add(Basket basket)
        {
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var p = await GetById(id);
            _context.Baskets.Remove(p);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Basket basket)
        {
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Basket> GetAll(Expression<Func<Basket, bool>> where = null)
        {
            var Baskets = _context.Baskets.AsQueryable();
            if (where != null)
            {
                Baskets = Baskets.Where(where);
            }
            return Baskets;
        }

        public async Task<Basket> GetById(int id)
        {
            return await _context.Baskets.FirstOrDefaultAsync(b => b.BasketId == id);
        }

        public async Task Update(Basket basket)
        {
            _context.Baskets.Update(basket);
            await _context.SaveChangesAsync();
        }
    }
}
