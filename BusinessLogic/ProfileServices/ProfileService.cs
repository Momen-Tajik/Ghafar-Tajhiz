using BusinessLogic.BasketServices;
using BusinessLogic.ProfileServices.Models;
using DataAccess.Enums;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.ProfileServices
{
    public class ProfileService
    {
        private readonly BasketService _basketService;

        public ProfileService(
            BasketService basketService)
        {
            _basketService = basketService;
        }


        public async Task<UserProfileViewModel>
            GetUserProfile(
                int userId,
                string userName,
                string phoneNumber,
                string? search,
                BasketStatus? status,
                string sort)
        {
            var orders =
                await _basketService.GetUserOrders(
                    userId,
                    search,
                    status,
                    sort);


            var lastOrder =
                await _basketService.GetLastUserOrder(
                    userId);


            return new UserProfileViewModel
            {
                UserId = userId,

                UserName =
                    string.IsNullOrWhiteSpace(userName)
                        ? "کاربر"
                        : userName,

                MobileNumber =
                    phoneNumber,

                Address =
                    lastOrder?.Address,

                Orders =
                    orders,

                Search =
                    search,

                Status =
                    status,

                Sort =
                    string.IsNullOrWhiteSpace(sort)
                        ? "paiddate"
                        : sort
            };
        }

        public async Task<bool> ResubmitReceipt(
    int userId,
    int basketId,
    IFormFile receipt)
        {
            return await _basketService.ResubmitReceipt(
                basketId,
                userId,
                receipt);
        }
    }
}