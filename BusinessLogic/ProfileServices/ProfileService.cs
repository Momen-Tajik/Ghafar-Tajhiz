using BusinessLogic.BasketServices;
using BusinessLogic.ProfileServices.Models;
using DataAccess.Enums;
using System.Threading.Tasks;

namespace BusinessLogic.ProfileServices
{
    public class ProfileService
    {
        private readonly BasketService _basketService;

        public ProfileService(BasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<UserProfileViewModel> GetUserProfile(int userId,string userName,string phoneNumber, string? search, BasketStatus? status, string sort)
        {
            var orders = await _basketService.GetUserOrders(userId, search, status, sort);
            var lastOrder = await _basketService.GetLastUserOrder(userId);

            return new UserProfileViewModel
            {
                UserId = userId,
                UserName = userName,
                MobileNumber = phoneNumber,
                Address = lastOrder?.Address,
                Orders = orders,
                Search = search,
                Status = status,
                Sort = sort
            };
        }
    }
}
