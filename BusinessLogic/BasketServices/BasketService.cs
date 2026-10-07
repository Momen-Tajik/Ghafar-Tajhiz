using BusinessLogic.BasketServices.Models;
using BusinessLogic.FileUpload;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.BasketServices
{
    public class BasketService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<BasketService> _logger;

        public BasketService(
            GhafarTajhizShopDbContext context,
            IFileUploadService fileUploadService,
            ILogger<BasketService> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        // =========================================================
        // Add Product To Basket
        // =========================================================

        public async Task<bool> AddToBasket(
            int productId,
            int qty,
            int userId)
        {
            if (productId <= 0 ||
                qty <= 0 ||
                userId <= 0)
            {
                return false;
            }

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
                    b.Status == BasketStatus.PendingPayment);

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = userId,
                    Status = BasketStatus.PendingPayment,
                    Created = DateTime.Now
                };

                _context.Baskets.Add(basket);
            }

            var basketItem = basket.BasketItems
                .FirstOrDefault(i =>
                    i.ProductId == productId);

            var currentQuantity =
                basketItem?.Qty ?? 0;

            var newQuantity =
                currentQuantity + qty;

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
                basketItem.UnitPrice = product.Price;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to add product {ProductId} to basket for user {UserId}",
                    productId,
                    userId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Get Current Basket
        // =========================================================

        public async Task<List<BasketItem>> GetUserBasket(
            int userId)
        {
            return await _context.BasketItems
                .AsNoTracking()
                .Where(i =>
                    i.Basket.UserId == userId &&
                    i.Basket.Status ==
                        BasketStatus.PendingPayment)
                .Include(i => i.Product)
                .ToListAsync();
        }

        // =========================================================
        // Submit Payment + Receipt
        // =========================================================

        public async Task<bool> Pay(
            string mobile,
            string address,
            IFormFile receipt,
            int userId)
        {
            if (userId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(mobile) ||
                string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            if (receipt == null ||
                receipt.Length == 0)
            {
                return false;
            }

            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    (
                        b.Status ==
                            BasketStatus.PendingPayment ||

                        b.Status ==
                            BasketStatus.PaymentRejected
                    ));

            if (basket == null ||
                basket.BasketItems.Count == 0)
            {
                return false;
            }

            // =====================================================
            // Check Stock
            // =====================================================

            foreach (var item in basket.BasketItems)
            {
                if (item.Product == null ||
                    !item.Product.IsAvailable ||
                    item.Qty <= 0 ||
                    item.Qty > item.Product.StockQuantity)
                {
                    return false;
                }
            }

            // =====================================================
            // Upload Receipt
            // =====================================================

            var receiptFileName =
                await _fileUploadService
                    .UploadReceiptAsync(receipt);

            if (string.IsNullOrWhiteSpace(receiptFileName))
                return false;

            // =====================================================
            // Update Order
            // =====================================================

            basket.Address =
                address.Trim();

            basket.MobileNumber =
                mobile.Trim();

            basket.ReceiptImage =
                receiptFileName;

            basket.ReceiptUploadedAt =
                DateTime.Now;

            basket.PaymentVerifiedAt = null;

            basket.PaymentRejectionReason = null;

            basket.PaidDate = null;

            basket.Status =
                BasketStatus.AwaitingPaymentVerification;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to submit payment for basket {BasketId}, user {UserId}",
                    basket.BasketId,
                    userId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Get User Orders
        // =========================================================

        public async Task<List<Basket>> GetUserOrders(
            int userId,
            string? search,
            BasketStatus? status,
            string sort = "paiddate")
        {
            var query = _context.Baskets
                .AsNoTracking()
                .Where(b =>
                    b.UserId == userId)
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b =>
                    b.Status == status.Value);
            }
            else
            {
                query = query.Where(b =>
                    b.Status !=
                        BasketStatus.PendingPayment);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch =
                    search.Trim();

                if (int.TryParse(
                    trimmedSearch,
                    out var basketId))
                {
                    query = query.Where(b =>
                        b.BasketId == basketId ||

                        (
                            b.MobileNumber != null &&
                            b.MobileNumber.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.Address != null &&
                            b.Address.Contains(
                                trimmedSearch)
                        ) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch))
                    );
                }
                else
                {
                    query = query.Where(b =>
                        (
                            b.MobileNumber != null &&
                            b.MobileNumber.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.Address != null &&
                            b.Address.Contains(
                                trimmedSearch)
                        ) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch))
                    );
                }
            }

            query = sort.Trim()
                .ToLowerInvariant() switch
            {
                "status" =>
                    query.OrderByDescending(
                        b => b.Status),

                "oldest" =>
                    query.OrderBy(
                        b => b.Created),

                _ =>
                    query.OrderByDescending(
                        b => b.Created)
            };

            return await query.ToListAsync();
        }

        // =========================================================
        // Get Admin Orders
        // =========================================================

        public async Task<List<AdminOrderDto>> GetAdminBskets(
            string? search,
            BasketStatus? status,
            string sort = "paiddate")
        {
            var query = _context.Baskets
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b =>
                    b.Status == status.Value);
            }
            else
            {
                query = query.Where(b =>
                    b.Status !=
                        BasketStatus.PendingPayment);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch =
                    search.Trim();

                if (int.TryParse(
                    trimmedSearch,
                    out var basketId))
                {
                    query = query.Where(b =>
                        b.BasketId == basketId ||

                        (
                            b.MobileNumber != null &&
                            b.MobileNumber.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.Address != null &&
                            b.Address.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.User != null &&
                            b.User.FullName.Contains(
                                trimmedSearch)
                        ) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch))
                    );
                }
                else
                {
                    query = query.Where(b =>
                        (
                            b.MobileNumber != null &&
                            b.MobileNumber.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.Address != null &&
                            b.Address.Contains(
                                trimmedSearch)
                        ) ||

                        (
                            b.User != null &&
                            b.User.FullName.Contains(
                                trimmedSearch)
                        ) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch))
                    );
                }
            }

            var resultQuery =
                query.Select(b => new AdminOrderDto
                {
                    AdminOrderId =
                        b.BasketId,

                    PaidDate =
                        b.PaidDate,

                    UserId =
                        b.UserId,

                    Address =
                        b.Address ?? string.Empty,

                    MobileNumber =
                        b.MobileNumber ?? string.Empty,

                    Status =
                        b.Status,

                    UserName =
                        b.User != null
                            ? b.User.FullName
                            : string.Empty,

                    Items =
                        b.BasketItems
                            .Select(i =>
                                i.Product.ProductName)
                            .ToList(),

                    ReceiptImage =
                        b.ReceiptImage,

                    ReceiptUploadedAt =
                        b.ReceiptUploadedAt,

                    PaymentVerifiedAt =
                        b.PaymentVerifiedAt,

                    PaymentRejectionReason =
                        b.PaymentRejectionReason
                });

            resultQuery =
                sort.Trim()
                    .ToLowerInvariant() switch
                {
                    "status" =>
                        resultQuery
                            .OrderByDescending(
                                x => x.Status),

                    "oldest" =>
                        resultQuery
                            .OrderBy(
                                x => x.PaidDate),

                    _ =>
                        resultQuery
                            .OrderByDescending(
                                x => x.PaidDate)
                };

            return await resultQuery.ToListAsync();
        }

        // =========================================================
        // Get Last User Order
        // =========================================================

        public async Task<Basket?> GetLastUserOrder(
            int userId)
        {
            return await _context.Baskets
                .AsNoTracking()
                .Where(b =>
                    b.UserId == userId &&
                    b.Status !=
                        BasketStatus.PendingPayment)
                .OrderByDescending(
                    b => b.Created)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // Basket Item Count
        // =========================================================

        public async Task<int> GetBasketItemCountAsync(
            int userId)
        {
            return await _context.BasketItems
                .Where(i =>
                    i.Basket.UserId == userId &&
                    i.Basket.Status ==
                        BasketStatus.PendingPayment)
                .SumAsync(i => i.Qty);
        }

        // =========================================================
        // Approve Payment
        // =========================================================

        public async Task<bool> ApprovePayment(
            int basketId)
        {
            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b =>
                    b.BasketId == basketId);

            if (basket == null)
                return false;

            if (basket.Status !=
                BasketStatus.AwaitingPaymentVerification)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                basket.ReceiptImage))
            {
                return false;
            }

            if (basket.BasketItems.Count == 0)
                return false;

            // =====================================================
            // Check Stock
            // =====================================================

            foreach (var item in basket.BasketItems)
            {
                if (item.Product == null ||
                    !item.Product.IsAvailable ||
                    item.Qty <= 0 ||
                    item.Qty > item.Product.StockQuantity)
                {
                    return false;
                }
            }

            // =====================================================
            // Decrease Stock
            // =====================================================

            foreach (var item in basket.BasketItems)
            {
                item.Product.StockQuantity -=
                    item.Qty;

                if (item.Product.StockQuantity == 0)
                {
                    item.Product.IsAvailable = false;
                }
            }

            // =====================================================
            // Update Payment Status
            // =====================================================

            basket.Status =
                BasketStatus.PaymentApproved;

            basket.PaidDate =
                DateTime.Now;

            basket.PaymentVerifiedAt =
                DateTime.Now;

            basket.PaymentRejectionReason =
                null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to approve payment for basket {BasketId}",
                    basketId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Reject Payment
        // =========================================================

        public async Task<bool> RejectPayment(
            int basketId,
            string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return false;

            var basket =
                await _context.Baskets
                    .FirstOrDefaultAsync(b =>
                        b.BasketId == basketId);

            if (basket == null)
                return false;

            if (basket.Status !=
                BasketStatus.AwaitingPaymentVerification)
            {
                return false;
            }

            basket.Status =
                BasketStatus.PaymentRejected;

            basket.PaymentRejectionReason =
                reason.Trim();

            basket.PaymentVerifiedAt =
                DateTime.Now;

            basket.PaidDate = null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to reject payment for basket {BasketId}",
                    basketId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Ship Order
        // =========================================================

        public async Task<bool> ShipOrder(
            int basketId)
        {
            var basket =
                await _context.Baskets
                    .FirstOrDefaultAsync(b =>
                        b.BasketId == basketId);

            if (basket == null)
                return false;

            if (basket.Status !=
                BasketStatus.PaymentApproved)
            {
                return false;
            }

            basket.Status =
                BasketStatus.Shipped;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to ship basket {BasketId}",
                    basketId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Cancel Order
        // =========================================================

        public async Task<bool> CancelOrder(
            int basketId)
        {
            var basket =
                await _context.Baskets
                    .FirstOrDefaultAsync(b =>
                        b.BasketId == basketId);

            if (basket == null)
                return false;

            if (basket.Status !=
                    BasketStatus.PendingPayment &&
                basket.Status !=
                    BasketStatus.AwaitingPaymentVerification &&
                basket.Status !=
                    BasketStatus.PaymentRejected)
            {
                return false;
            }

            basket.Status =
                BasketStatus.Cancelled;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to cancel basket {BasketId}",
                    basketId);

                return false;
            }

            return true;
        }

        // =========================================================
        // Get Admin Order Details
        // =========================================================

        public async Task<AdminOrderDetailDto?>
            GetAdminOrderDetail(
                int basketId)
        {
            return await _context.Baskets
                .AsNoTracking()
                .Where(b =>
                    b.BasketId == basketId)
                .Select(b =>
                    new AdminOrderDetailDto
                    {
                        BasketId =
                            b.BasketId,

                        UserId =
                            b.UserId,

                        UserName =
                            b.User != null
                                ? b.User.FullName
                                : string.Empty,

                        MobileNumber =
                            b.MobileNumber ??
                            string.Empty,

                        Address =
                            b.Address ??
                            string.Empty,

                        Status =
                            b.Status,

                        Created =
                            b.Created,

                        PaidDate =
                            b.PaidDate,

                        ReceiptUploadedAt =
                            b.ReceiptUploadedAt,

                        PaymentVerifiedAt =
                            b.PaymentVerifiedAt,

                        ReceiptImage =
                            b.ReceiptImage,

                        PaymentRejectionReason =
                            b.PaymentRejectionReason,

                        Items =
                            b.BasketItems
                                .Select(i =>
                                    new AdminOrderDetailItemDto
                                    {
                                        ProductId =
                                            i.ProductId,

                                        ProductName =
                                            i.Product.ProductName,

                                        ImageUrl =
                                            i.Product.ImageUrl,

                                        Qty =
                                            i.Qty,

                                        UnitPrice =
                                            i.UnitPrice
                                    })
                                .ToList()
                    })
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // Resubmit Receipt
        // =========================================================

        public async Task<bool> ResubmitReceipt(
            int basketId,
            int userId,
            IFormFile receipt)
        {
            if (basketId <= 0)
                return false;

            if (receipt == null || receipt.Length == 0)
                return false;

            var basket = await _context.Baskets
                .FirstOrDefaultAsync(b =>
                    b.BasketId == basketId &&
                    b.UserId == userId &&
                    b.Status == BasketStatus.PaymentRejected);

            if (basket == null)
                return false;

            var receiptFileName =
                await _fileUploadService
                    .UploadReceiptAsync(receipt);

            if (string.IsNullOrWhiteSpace(receiptFileName))
                return false;

            basket.ReceiptImage =
                receiptFileName;

            basket.ReceiptUploadedAt =
                DateTime.Now;

            basket.PaymentVerifiedAt = null;

            basket.PaymentRejectionReason = null;

            basket.PaidDate = null;

            basket.Status =
                BasketStatus.AwaitingPaymentVerification;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to resubmit receipt for basket {BasketId}, user {UserId}",
                    basketId,
                    userId);

                return false;
            }

            return true;
        }
    }
}