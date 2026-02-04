using BusinessLogic.BasketServices.Models;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.BasketItemRepo;
using DataAccess.Repositories.BasketRepo;
using DataAccess.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BasketServices
{
    public class BasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBasketItemRepository _basketItemRepository;

        public BasketService(IBasketRepository basketRepository, IProductRepository productRepository, IBasketItemRepository basketItemRepository)
        {
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _basketItemRepository = basketItemRepository;
        }

        public async Task<bool> AddToBasket(int productId, int qty, int userId)
        {
            var basket = await _basketRepository
                .GetAll(a => a.UserId == userId && a.Status == BasketStatus.Pending)
                .FirstOrDefaultAsync();

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = userId,
                    Status = BasketStatus.Pending,
                    Created = DateTime.Now,
                };

                await _basketRepository.Add(basket);
            }

            var product = await _productRepository.GetById(productId);

            var basketItem = await _basketItemRepository
                .GetAll(a => a.BasketId == basket.BasketId && a.ProductId == productId)
                .FirstOrDefaultAsync();

            if (basketItem != null)
            {
                basketItem.Qty += qty;
                basketItem.UnitPrice = product.Price * basketItem.Qty;

                await _basketItemRepository.Update(basketItem);
            }
            else
            {
                basketItem = new BasketItem
                {
                    BasketId = basket.BasketId,
                    ProductId = product.ProductId,
                    Qty = qty,
                    UnitPrice = product.Price * qty,
                    Created = DateTime.Now
                };

                await _basketItemRepository.Add(basketItem);
            }

            return true;
        }


        public async Task<List<BasketItem>> GetUserBasket(int userId)
        {
            var basketItems = await _basketItemRepository.GetAll(a => a.Basket.UserId == userId && a.Basket.Status == BasketStatus.Pending)
                .Include(a => a.Basket).Include(a => a.Product).ToListAsync();
            return basketItems;
        }

        public async Task<bool> Pay(string mobile, string address, int userId)
        {
            var basket = await _basketRepository.GetAll(a => a.UserId == userId && a.Status == BasketStatus.Pending).FirstOrDefaultAsync();

            if (basket == null)
                return false;

            basket.Address = address;
            basket.PaidDate = DateTime.Now;
            basket.Status = BasketStatus.Paid;
            basket.MobileNumber = mobile;

            await _basketRepository.Update(basket);
            return true;
        }

        public async Task<List<Basket>> GetUserBskets(int userId)
        {
            var baskets = await _basketRepository.GetAll(a => a.UserId == userId && a.Status != BasketStatus.Pending)
                .Include(a => a.BasketItems).ThenInclude(a => a.Product).AsNoTracking().ToListAsync();
            return baskets;
        }


        public async Task<List<AdminOrderDto>> GetAdminBskets(string? search, string sort = "paiddate")
        {
            var query = _basketRepository.GetAll(b => b.Status != BasketStatus.Pending)
                .Include(b => b.User)
                .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Product)
                .Select(b => new AdminOrderDto
                {
                    AdminOrderId = b.BasketId,
                    PaidDate = b.PaidDate!.Value,
                    UserId = b.UserId,
                    Address = b.Address,
                    MobileNumber = b.MobileNumber,
                    Status = b.Status,
                    UserName = b.User.UserName,
                    items = b.BasketItems.Select(i => i.Product.ProductName).ToList()
                });

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.UserName.Contains(search) ||
                    p.MobileNumber.Contains(search) ||
                    p.Address.Contains(search));
            }

            query = sort.ToLower() switch
            {
                "status" => query.OrderByDescending(p => p.Status),
                "paiddate" => query.OrderByDescending(p => p.PaidDate),
                _ => query.OrderByDescending(p => p.PaidDate)
            };

            return await query.AsNoTracking().ToListAsync();
        }


        public async Task<List<Basket>> GetUserOrders(int userId, string? search, BasketStatus? status, string sort = "paiddate")
        {
            var query = _basketRepository
                .GetAll(b => b.UserId == userId)
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);

            //if (status.HasValue)
            //{
            //    query = query.Where(b => b.Status == status);

            //}

            //if (!string.IsNullOrEmpty(search))
            //{
            //    query = query.Where(b =>
            //        b.BasketId.ToString().Contains(search) ||
            //        b.MobileNumber.Contains(search) ||
            //        b.Address.Contains(search) ||
            //        b.BasketItems.Any(i => i.Product.ProductName.Contains(search))
            //    );
            //}

            //query = sort.ToLower() switch
            //{
            //    "status" => query.OrderByDescending(b => b.Status),
            //    "oldest" => query.OrderBy(b => b.PaidDate),
            //    "paiddate" => query.OrderByDescending(b => b.PaidDate),
            //    _ => query.OrderByDescending(b => b.PaidDate)
            //};

            return await query.AsNoTracking().OrderByDescending(a=>a.Created).ToListAsync();
        }

        public async Task<Basket?> GetLastUserOrder(int userId)
        {
            return await _basketRepository
                .GetAll(b => b.UserId == userId && b.Status != BasketStatus.Pending)
                .OrderByDescending(b => b.PaidDate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

    }
}
