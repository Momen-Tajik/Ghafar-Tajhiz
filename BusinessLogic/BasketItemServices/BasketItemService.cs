using DataAccess.Data;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.BasketItemServices
{
    public class BasketItemService
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketItemService(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RemoveBasketItem(
            int basketItemId,
            int userId)
        {
            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(i =>
                    i.BasketItemId == basketItemId &&
                    i.Basket.UserId == userId &&
                    i.Basket.Status == BasketStatus.PendingPayment);

            if (basketItem == null)
                return false;

            _context.BasketItems.Remove(basketItem);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}