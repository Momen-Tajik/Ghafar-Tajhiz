using BusinessLogic.BasketItemServices;
using BusinessLogic.BasketServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BasketService _basketService;
        private readonly BasketItemService _basketItemService;
        public ProfileController(BasketService basketService, BasketItemService basketItemService)
        {
            _basketService = basketService;
            _basketItemService = basketItemService;
        }
        public async Task<IActionResult> Index()
        {
            var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _basketService.GetUserBskets(Convert.ToInt32(userId));
            return View(data);
        }
    }
}
