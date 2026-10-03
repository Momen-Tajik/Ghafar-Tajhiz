using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.BasketRepo
{
    public class BasketRepository : IBasketRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Basket>> GetAllAsync()
        {
            return await _context.Baskets
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Basket?> GetByIdAsync(int id)
        {
            return await _context.Baskets
                .FirstOrDefaultAsync(b => b.BasketId == id);
        }

        public async Task AddAsync(Basket basket)
        {
            await _context.Baskets.AddAsync(basket);
        }

        public void Update(Basket basket)
        {
            _context.Baskets.Update(basket);
        }

        public void Delete(Basket basket)
        {
            _context.Baskets.Remove(basket);
        }
    }
}