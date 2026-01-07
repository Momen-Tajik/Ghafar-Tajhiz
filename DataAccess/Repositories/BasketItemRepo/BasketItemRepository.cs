using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BasketItemRepo
{
    public class BasketItemRepository :IBasketItemRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketItemRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task Add(BasketItem basketItem)
        {
            _context.BasketItems.Add(basketItem);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var p = await GetById(id);
            _context.BasketItems.Remove(p);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(BasketItem basketItem)
        {
            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();
        }

        public IQueryable<BasketItem> GetAll(Expression<Func<BasketItem, bool>> where = null)
        {
            var basketItems = _context.BasketItems.AsQueryable();
            if (where != null)
            {
                basketItems = basketItems.Where(where);
            }
            return basketItems;
        }

        public async Task<BasketItem> GetById(int id)
        {
            return await _context.BasketItems.FirstOrDefaultAsync(bI=>bI.BasketItemId==id);
        }

        public async Task Update(BasketItem basketItem)
        {
            _context.BasketItems.Update(basketItem);
            await _context.SaveChangesAsync();
        }
    }
}
