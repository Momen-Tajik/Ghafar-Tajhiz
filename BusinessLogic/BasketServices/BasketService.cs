using BusinessLogic.BasketServices.Models;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.BasketServices
{
    public class BasketService
    {
        private readonly GhafarTajhizShopDbContext _context;

        public BasketService(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToBasket(int productId, int qty, int userId)
        {
            if (qty <= 0)
                return false;

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.IsAvailable);

            if (product == null)
                return false;

            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.Status == BasketStatus.Pending);

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = userId,
                    Status = BasketStatus.Pending,
                    Created = DateTime.Now
                };

                _context.Baskets.Add(basket);
            }

            var basketItem = basket.BasketItems
                .FirstOrDefault(i => i.ProductId == productId);

            var newQuantity = (basketItem?.Qty ?? 0) + qty;

            if (newQuantity > product.StockQuantity)
                return false;

            if (basketItem == null)
            {
                basketItem = new BasketItem
                {
                    ProductId = product.ProductId,
                    Qty = qty,
                    UnitPrice = product.Price,
                    Created = DateTime.Now
                };

                basket.BasketItems.Add(basketItem);
            }
            else
            {
                basketItem.Qty = newQuantity;

                // قیمت یک واحد، نه قیمت کل
                basketItem.UnitPrice = product.Price;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BasketItem>> GetUserBasket(int userId)
        {
            return await _context.BasketItems
                .AsNoTracking()
                .Where(i =>
                    i.Basket.UserId == userId &&
                    i.Basket.Status == BasketStatus.Pending)
                .Include(i => i.Product)
                .ToListAsync();
        }

        public async Task<bool> Pay(
            string mobile,
            string address,
            int userId)
        {
            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.Status == BasketStatus.Pending);

            if (basket == null || basket.BasketItems.Count == 0)
                return false;

            foreach (var item in basket.BasketItems)
            {
                if (!item.Product.IsAvailable ||
                    item.Qty > item.Product.StockQuantity)
                {
                    return false;
                }
            }

            basket.Address = address;
            basket.MobileNumber = mobile;
            basket.PaidDate = DateTime.Now;
            basket.Status = BasketStatus.Paid;

            foreach (var item in basket.BasketItems)
            {
                item.Product.StockQuantity -= item.Qty;

                if (item.Product.StockQuantity == 0)
                    item.Product.IsAvailable = false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Basket>> GetUserOrders(
            int userId,
            string? search,
            BasketStatus? status,
            string sort = "paiddate")
        {
            var query = _context.Baskets
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }
            else
            {
                query = query.Where(b => b.Status != BasketStatus.Pending);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch = search.Trim();

                if (int.TryParse(trimmedSearch, out var basketId))
                {
                    query = query.Where(b =>
                        b.BasketId == basketId ||
                        (b.MobileNumber != null &&
                         b.MobileNumber.Contains(trimmedSearch)) ||
                        (b.Address != null &&
                         b.Address.Contains(trimmedSearch)) ||
                        b.BasketItems.Any(i =>
                            i.Product.ProductName.Contains(trimmedSearch)));
                }
                else
                {
                    query = query.Where(b =>
                        (b.MobileNumber != null &&
                         b.MobileNumber.Contains(trimmedSearch)) ||
                        (b.Address != null &&
                         b.Address.Contains(trimmedSearch)) ||
                        b.BasketItems.Any(i =>
                            i.Product.ProductName.Contains(trimmedSearch)));
                }
            }

            query = sort.Trim().ToLowerInvariant() switch
            {
                "status" =>
                    query.OrderByDescending(b => b.Status),

                "oldest" =>
                    query.OrderBy(b => b.PaidDate),

                _ =>
                    query.OrderByDescending(b => b.PaidDate)
            };

            return await query.ToListAsync();
        }

        public async Task<List<AdminOrderDto>> GetAdminBskets(
            string? search,
            string sort = "paiddate")
        {
            var query = _context.Baskets
                .AsNoTracking()
                .Where(b => b.Status != BasketStatus.Pending)
                .Select(b => new AdminOrderDto
                {
                    AdminOrderId = b.BasketId,
                    PaidDate = b.PaidDate ?? DateTime.MinValue,
                    UserId = b.UserId,
                    Address = b.Address ?? string.Empty,
                    MobileNumber = b.MobileNumber ?? string.Empty,
                    Status = b.Status,
                    UserName = b.User!.FullName ?? string.Empty,
                    items = b.BasketItems
                        .Select(i => i.Product.ProductName)
                        .ToList()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch = search.Trim();

                query = query.Where(o =>
                    o.UserName.Contains(trimmedSearch) ||
                    o.MobileNumber.Contains(trimmedSearch) ||
                    o.Address.Contains(trimmedSearch) ||
                    o.items.Any(p => p.Contains(trimmedSearch)));
            }

            query = sort.Trim().ToLowerInvariant() switch
            {
                "status" =>
                    query.OrderByDescending(o => o.Status),

                "oldest" =>
                    query.OrderBy(o => o.PaidDate),

                _ =>
                    query.OrderByDescending(o => o.PaidDate)
            };

            return await query.ToListAsync();
        }

        public async Task<Basket?> GetLastUserOrder(int userId)
        {
            return await _context.Baskets
                .AsNoTracking()
                .Where(b =>
                    b.UserId == userId &&
                    b.Status != BasketStatus.Pending)
                .OrderByDescending(b => b.PaidDate)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetBasketItemCountAsync(int userId)
        {
            return await _context.BasketItems
                .Where(i =>
                    i.Basket.UserId == userId &&
                    i.Basket.Status == BasketStatus.Pending)
                .SumAsync(i => i.Qty);
        }

        public async Task<bool> SetState(int basketId, bool shipped)
        {
            var basket = await _context.Baskets
                .FirstOrDefaultAsync(b => b.BasketId == basketId);

            if (basket == null)
                return false;

            if (basket.Status != BasketStatus.Paid)
                return false;

            basket.Status = shipped
                ? BasketStatus.Shipped
                : BasketStatus.Cancelled;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}