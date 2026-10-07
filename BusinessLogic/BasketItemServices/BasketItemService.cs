using DataAccess.Data;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.BasketItemServices
{
    public class BasketItemService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly ILogger<BasketItemService> _logger;

        public BasketItemService(
            GhafarTajhizShopDbContext context,
            ILogger<BasketItemService> logger)
        {
            _context = context;
            _logger = logger;
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to remove basket item {BasketItemId} for user {UserId}",
                    basketItemId,
                    userId);

                return false;
            }

            return true;
        }
    }
}