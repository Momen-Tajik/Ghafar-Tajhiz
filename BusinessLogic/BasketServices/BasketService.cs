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
            //var baskets = await _basketRepository.GetAll( a=> a.Status != BasketStatus.Pending)
            //    .Include(a => a.BasketItems).ThenInclude(a => a.Product).AsNoTracking().ToListAsync();

            var baskets =await _basketRepository.GetAll()
                .Include(u=>u.User)
                .Include(bi=>bi.BasketItems)
                .ThenInclude(p=>p.Product)
                .Select(s=>new AdminOrderDto()
                {
                    AdminOrderId = s.BasketId,
                    PaidDate = s.PaidDate.Value,
                    UserId = s.UserId,
                    Address = s.Address,
                    MobileNumber = s.MobileNumber,
                    Status = s.Status,
                    UserName = s.User.UserName,
                    items = s.BasketItems.Select(s=> s.Product.ProductName).ToList()

                }).AsNoTracking().ToListAsync();

            //Filtering
            switch (sort.ToLower())
            {
                case "paiddate":
                    baskets = (List<AdminOrderDto>)baskets.OrderByDescending(p => p.Status);
                    break;

                case "status":
                    baskets = (List<AdminOrderDto>)baskets.OrderByDescending(p => p.Status);
                    break;


                default:
                    baskets = (List<AdminOrderDto>)baskets.OrderByDescending(p => p.PaidDate);
                    break;
            }

            // Searching
            if (!search.IsNullOrEmpty())
            {
                baskets = (List<AdminOrderDto>)baskets.Where(p =>
                    p.UserName.Contains(search) ||
                    p.MobileNumber.Contains(search) ||
                    //p.PaidDate.ToString().Contains(search) ||
                    p.Address.Contains(search));
            }
            return baskets;
        }
    }
}
