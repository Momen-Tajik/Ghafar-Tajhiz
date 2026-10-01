using BusinessLogic.BasketServices.Models;
using BusinessLogic.FileUpload;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.BasketServices
{
    public class BasketService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public BasketService(
            GhafarTajhizShopDbContext context,
            IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }


        // =========================================================
        // Add Product To Basket
        // =========================================================

        public async Task<bool> AddToBasket(
            int productId,
            int qty,
            int userId)
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

            var newQuantity =
                (basketItem?.Qty ?? 0) + qty;


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

                // قیمت واحد
                basketItem.UnitPrice = product.Price;
            }


            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // Get Current Basket
        // =========================================================

        public async Task<List<BasketItem>> GetUserBasket(int userId)
        {
            return await _context.BasketItems
                .AsNoTracking()
                .Where(i =>
                    i.Basket.UserId == userId &&
                    i.Basket.Status == BasketStatus.PendingPayment)
                .Include(i => i.Product)
                .ToListAsync();
        }


        // =========================================================
        // Submit Payment / Upload Receipt
        // =========================================================
        // فعلاً این متد اطلاعات سفارش را ذخیره می‌کند
        // و وضعیت را به AwaitingPaymentVerification می‌برد.
        //
        // ذخیره فایل رسید باید در بخش Upload Receipt انجام شود.
        // =========================================================
        public async Task<bool> Pay(
            string mobile,
            string address,
            IFormFile receipt,
            int userId)
        {
            if (receipt == null || receipt.Length == 0)
                return false;


            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.Status == BasketStatus.PendingPayment);


            if (basket == null ||
                basket.BasketItems.Count == 0)
            {
                return false;
            }


            // بررسی موجودی
            foreach (var item in basket.BasketItems)
            {
                if (item.Product == null ||
                    !item.Product.IsAvailable ||
                    item.Qty > item.Product.StockQuantity)
                {
                    return false;
                }
            }


            // آپلود رسید
            var receiptFileName =
                await _fileUploadService.UploadReceiptAsync(receipt);


            basket.Address = address;

            basket.MobileNumber = mobile;

            basket.ReceiptImage =
                receiptFileName;

            basket.ReceiptUploadedAt =
                DateTime.Now;

            basket.PaymentVerifiedAt = null;

            basket.PaymentRejectionReason = null;

            basket.PaidDate = null;

            basket.Status =
                BasketStatus.AwaitingPaymentVerification;


            await _context.SaveChangesAsync();


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
                .Where(b => b.UserId == userId)
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();


            // وقتی فیلتر وضعیت انتخاب نشده
            // سبد موقت را نمایش نده
            if (status.HasValue)
            {
                query = query.Where(b =>
                    b.Status == status.Value);
            }
            else
            {
                query = query.Where(b =>
                    b.Status != BasketStatus.PendingPayment);
            }


            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch = search.Trim();


                if (int.TryParse(
                    trimmedSearch,
                    out var basketId))
                {
                    query = query.Where(b =>
                        b.BasketId == basketId ||

                        (b.MobileNumber != null &&
                         b.MobileNumber.Contains(trimmedSearch)) ||

                        (b.Address != null &&
                         b.Address.Contains(trimmedSearch)) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch)));
                }
                else
                {
                    query = query.Where(b =>

                        (b.MobileNumber != null &&
                         b.MobileNumber.Contains(trimmedSearch)) ||

                        (b.Address != null &&
                         b.Address.Contains(trimmedSearch)) ||

                        b.BasketItems.Any(i =>
                            i.Product.ProductName
                                .Contains(trimmedSearch)));
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
            string sort = "paiddate")
        {
            var query = _context.Baskets
                .AsNoTracking()
                .Where(b =>
                    b.Status != BasketStatus.PendingPayment)
                .Select(b => new AdminOrderDto
                {
                    AdminOrderId = b.BasketId,

                    PaidDate = b.PaidDate,

                    UserId = b.UserId,

                    Address = b.Address ?? string.Empty,

                    MobileNumber = b.MobileNumber ?? string.Empty,

                    Status = b.Status,

                    UserName = b.User!.FullName ?? string.Empty,

                    Items = b.BasketItems
                .Select(i => i.Product.ProductName)
                .ToList(),

                    ReceiptImage = b.ReceiptImage,

                    ReceiptUploadedAt = b.ReceiptUploadedAt,

                    PaymentVerifiedAt = b.PaymentVerifiedAt,

                    PaymentRejectionReason = b.PaymentRejectionReason
                });


            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch =
                    search.Trim();

                query = query.Where(o =>
                    o.UserName.Contains(trimmedSearch) ||
                    o.MobileNumber.Contains(trimmedSearch) ||
                    o.Address.Contains(trimmedSearch) ||
                    o.Items.Any(p =>
                        p.Contains(trimmedSearch)));
            }


            query = sort.Trim()
                .ToLowerInvariant() switch
            {
                "status" =>
                    query.OrderByDescending(
                        o => o.Status),

                "oldest" =>
                    query.OrderBy(
                        o => o.PaidDate),

                _ =>
                    query.OrderByDescending(
                        o => o.PaidDate)
            };


            return await query.ToListAsync();
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
                    b.Status != BasketStatus.PendingPayment)
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
        // Approve / Reject Payment
        // =========================================================

        public async Task<bool> SetState(
            int basketId,
            bool approved)
        {
            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b =>
                    b.BasketId == basketId);


            if (basket == null)
                return false;


            // فقط سفارش‌هایی که منتظر بررسی هستند
            // قابل تأیید یا رد هستند.
            if (basket.Status !=
                BasketStatus.AwaitingPaymentVerification)
            {
                return false;
            }


            // =====================================================
            // Reject Payment
            // =====================================================

            if (!approved)
            {
                basket.Status =
                    BasketStatus.PaymentRejected;

                await _context.SaveChangesAsync();

                return true;
            }


            // =====================================================
            // Approve Payment
            // =====================================================

            foreach (var item in basket.BasketItems)
            {
                if (item.Product == null ||
                    !item.Product.IsAvailable ||
                    item.Qty > item.Product.StockQuantity)
                {
                    return false;
                }
            }


            // کم کردن موجودی فقط بعد از تأیید پرداخت
            foreach (var item in basket.BasketItems)
            {
                item.Product.StockQuantity -= item.Qty;

                if (item.Product.StockQuantity == 0)
                {
                    item.Product.IsAvailable = false;
                }
            }


            basket.Status =
                BasketStatus.PaymentApproved;

            basket.PaidDate =
                DateTime.Now;


            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // Ship Order
        // =========================================================

        public async Task<bool> ShipOrder(
            int basketId)
        {
            var basket = await _context.Baskets
                .FirstOrDefaultAsync(b =>
                    b.BasketId == basketId);


            if (basket == null)
                return false;


            // فقط سفارش تأیید شده قابل ارسال است
            if (basket.Status !=
                BasketStatus.PaymentApproved)
            {
                return false;
            }


            basket.Status =
                BasketStatus.Shipped;


            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // Cancel Order
        // =========================================================

        public async Task<bool> CancelOrder(
            int basketId)
        {
            var basket = await _context.Baskets
                .FirstOrDefaultAsync(b =>
                    b.BasketId == basketId);


            if (basket == null)
                return false;


            if (basket.Status == BasketStatus.Shipped ||
                basket.Status == BasketStatus.Cancelled)
            {
                return false;
            }


            basket.Status =
                BasketStatus.Cancelled;


            await _context.SaveChangesAsync();

            return true;
        }
        
        public async Task<AdminOrderDetailDto?> GetAdminOrderDetail(int basketId)
        {
            return await _context.Baskets
                .AsNoTracking()
                .Where(b => b.BasketId == basketId)
                .Select(b => new AdminOrderDetailDto
                {
                    BasketId = b.BasketId,

                    UserId = b.UserId,

                    UserName = b.User!.FullName ?? string.Empty,

                    MobileNumber = b.MobileNumber ?? string.Empty,

                    Address = b.Address ?? string.Empty,

                    Status = b.Status,

                    Created = b.Created,

                    PaidDate = b.PaidDate,

                    ReceiptUploadedAt = b.ReceiptUploadedAt,

                    PaymentVerifiedAt = b.PaymentVerifiedAt,

                    ReceiptImage = b.ReceiptImage,

                    PaymentRejectionReason =
                        b.PaymentRejectionReason,

                    Items = b.BasketItems
                        .Select(i => new AdminOrderDetailItemDto
                        {
                            ProductId = i.ProductId,

                            ProductName =
                                i.Product.ProductName,

                            ImageUrl =
                                i.Product.ImageUrl,

                            Qty = i.Qty,

                            UnitPrice = i.UnitPrice
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}