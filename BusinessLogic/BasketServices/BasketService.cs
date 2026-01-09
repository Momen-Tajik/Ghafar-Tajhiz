using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.BasketItemRepo;
using DataAccess.Repositories.BasketRepo;
using DataAccess.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            
                var basket = new Basket();
                basket = await _basketRepository.GetAll(a => a.UserId == userId && a.Status == BasketStatus.Pending).FirstOrDefaultAsync();

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
            else 
            {
                var product =await _productRepository.GetById(productId);

                var basketItem = new BasketItem() 
                {
                    BasketId=basket.BasketId,
                    Qty=qty,
                    ProductId=product.ProductId,
                    Created=DateTime.Now,
                    Price=product.Price * qty,
                }; 
                await _basketItemRepository.Add(basketItem);
            }
            return true;
        }


        public async Task<List<BasketItem>> GetUserBasket(int userId)
        {
            var basketItems=await _basketItemRepository.GetAll(a=>a.Basket.UserId ==userId && a.Basket.Status== BasketStatus.Pending)
                .Include(a=>a.Basket).Include(a=>a.Product).ToListAsync();
            return basketItems;
        }

        public async Task<bool> Pay(string mobile, string address, int userId) 
        {
            var basket= await _basketRepository.GetAll(a=>a.UserId == userId && a.Status==BasketStatus.Pending).FirstOrDefaultAsync();

            if (basket == null)
                return false;

            basket.Address = address;
            basket.PaidDate = DateTime.Now;
            basket.Status = BasketStatus.Paid;
            basket.MobileNumber = mobile;
            
            await _basketRepository.Update(basket);
            return true;
        }
    }
}
