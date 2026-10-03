using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.BasketItemRepo
{
    public class BasketItemRepository : IBasketItemRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketItemRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BasketItem>> GetAllAsync()
        {
            return await _context.BasketItems
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BasketItem?> GetByIdAsync(int id)
        {
            return await _context.BasketItems
                .FirstOrDefaultAsync(b => b.BasketItemId == id);
        }

        public async Task AddAsync(BasketItem basketItem)
        {
            await _context.BasketItems.AddAsync(basketItem);
        }

        public void Update(BasketItem basketItem)
        {
            _context.BasketItems.Update(basketItem);
        }

        public void Delete(BasketItem basketItem)
        {
            _context.BasketItems.Remove(basketItem);
        }
    }
}